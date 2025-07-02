using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AspNetControllerApiBoilerPlate.Application.DTOs.User;
using AspNetControllerApiBoilerPlate.Infrastructure;
using AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Entities;
using AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspNetControllerApiBoilerPlate.Application.Services.Authentication;

public class AuthenticationService(
    SignInManager<User> signInManager,
    UserRepository userRepository,
    IConfiguration configuration,
    EmailSender emailSender)
{
    public async Task RegisterUserAsync(UserRegisterDto userRegisterDto, IUrlHelper url, string requestScheme)
    {
        var user = new User
        {
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
            Email = userRegisterDto.Email,
            UserName = userRegisterDto.Email
        };
        var createUserResponse = await userRepository.CreateAsync(user, userRegisterDto.Password);

        var emailValidationToken = await userRepository.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = url.Action("ConfirmEmail", "Authentication",
            new { userId = user.Id, code = emailValidationToken },
            protocol: requestScheme);

        await emailSender.SendEmailAsync(user.Email, "Confirm your account",
            "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
    }

    public async Task<User?> LogUserInByEmailAsync(UserLoginDto userLoginDto)
    {
        var user = await userRepository.FindByEmailAsync(userLoginDto.Email);
        if (user == null) return null;

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

        return checkPasswordResult.IsNotAllowed
            ? null
            : user;
    }

    // TODO: Write methods (see controller)

    public string GenerateJwtToken(User user)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(
            configuration["Authentication:Jwt:RsaPrivateKey"]!); // Use the rsa private key to sign the JWT

        var signingCredentials =
            new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var userClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email,
                user.Email!) // Never null since it's required for user registration (is it really needed here ?)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Authentication:Jwt:Authority"],
            audience: configuration["Authentication:Jwt:Audience"],
            claims: userClaims,
            expires: DateTime.UtcNow.AddMinutes(30), // Use UtcNow for better handling across time zones
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }

    public async Task GenerateTwoFactorTokenAndSendItToUserViaEmailAsync(User user)
    {
        var a2FValidationCode = await userRepository.GenerateTwoFactorTokenAsync(user, "Email");

        await emailSender.SendEmailAsync(user.Email!, "2AF: Validation token",
            $"Here's your token to log in the application: <b>{a2FValidationCode}</b>");
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenToVerify)
    {
        return await userRepository.VerifyTwoFactorTokenAsync(user, "Email", tokenToVerify);
    }
}