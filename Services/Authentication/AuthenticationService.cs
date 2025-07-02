using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Server.DTOs.User;
using Server.Models;

namespace Server.Services.Authentication;

public class AuthenticationService(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IConfiguration configuration,
    EmailSender emailSender)
{
    public async Task<User?> LogUserInByEmailAsync(UserLoginDto userLoginDto)
    {
        var user = await userManager.FindByEmailAsync(userLoginDto.Email);
        if (user == null) return null;

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

        return checkPasswordResult.IsNotAllowed
            ? null
            : user;
    }

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
        var a2FValidationCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

        await emailSender.SendEmailAsync(user.Email!, "2AF: Validation token",
            $"Here's your token to log in the application: <b>{a2FValidationCode}</b>");
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenToVerify)
    {
        return await userManager.VerifyTwoFactorTokenAsync(user, "Email", tokenToVerify);
    }
}