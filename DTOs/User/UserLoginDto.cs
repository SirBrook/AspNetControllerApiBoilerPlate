using Microsoft.Build.Framework;

namespace AspNetControllerApiBoilerPlate.DTOs.User;

public class UserLoginDto
{
    [Required] public required string Password { get; init; }
    [Required] public required string Email { get; init; }
}