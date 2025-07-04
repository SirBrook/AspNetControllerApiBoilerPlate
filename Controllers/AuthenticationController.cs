using AspNetControllerApiBoilerPlate.DTOs.User;
using AspNetControllerApiBoilerPlate.Models;
using AspNetControllerApiBoilerPlate.Services;
using AspNetControllerApiBoilerPlate.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetControllerApiBoilerPlate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(
        UserManager<User> userManager,
        AuthenticationService authenticationService,
        EmailSender emailSender)
        : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto)
        {
            var user = new User
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                Email = userRegisterDto.Email,
                UserName = userRegisterDto.Email
            };
            var createUserResponse = await userManager.CreateAsync(user, userRegisterDto.Password);
            if (!createUserResponse.Succeeded) return BadRequest(createUserResponse.Errors);

            var emailValidationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Action("ConfirmEmail", "Authentication",
                new { userId = user.Id, code = emailValidationToken },
                protocol: Request.Scheme);


            await emailSender.SendEmailAsync(user.Email, "Confirm your account",
                "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return Ok("The app is working fine");
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Could not confirm your account");

            var confirmEmailResponse = await userManager.ConfirmEmailAsync(user, code);
            if (!confirmEmailResponse.Succeeded)
                return BadRequest("Could not confirm your account for the given user id and code");

            return Ok("Account confirmed");
        }

        [AllowAnonymous]
        [HttpPost("login/2af/generate-token")]
        public async Task<IActionResult> LogInGenerateTwoFactorToken([FromBody] UserLoginDto userLoginDto)
        {
            // 1) Check username / password match
            var user = await authenticationService.LogUserInByEmailAsync(userLoginDto);
            if (user == null) return Unauthorized("Unauthorized: Wrong username password association.");

            // 2) Generate the 2AF token and send it by email, return the userId to push the user id in the url query front side
            await authenticationService.GenerateTwoFactorTokenAndSendItToUserViaEmailAsync(user);

            return Ok(user.Id);
        }

        [AllowAnonymous]
        [HttpPost("login/2af/verify-token")]
        public async Task<IActionResult> LogUserInJwtBasedWithTwoFactorToken([FromQuery] string userId,
            string twoFactorAuthTokenToVerify)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Incorrect user id");

            var isTokenValid = await authenticationService.VerifyTwoFactorTokenAsync(user, twoFactorAuthTokenToVerify);
            if (!isTokenValid) return Unauthorized("Two factor authentication token is incorrect.");

            var token = authenticationService.GenerateJwtToken(user);
            Response.Cookies.Append("access_token", token.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            return Ok("JWT issued and stored in HttpOnly cookie");
        }

        [HttpGet("logout")]
        public IActionResult LogUserOutJwtBased()
        {
            Response.Cookies.Append("access_token", string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            return Ok("Logged out successfully from JWT cookie http only");
        }

        [HttpGet("require-auth-test")]
        public IActionResult SimpleTest()
        {
            return Ok("The app is working fine");
        }
    }
}