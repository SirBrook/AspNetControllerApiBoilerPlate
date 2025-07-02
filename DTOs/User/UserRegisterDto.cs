using Microsoft.Build.Framework;

namespace Server.DTOs.User;

public class UserRegisterDto
{
    [Required] public required string FirstName { get; init; }
    [Required] public required string LastName { get; init; }
    [Required] public required string Email { get; init; }
    [Required] public required string Password { get; init; }
}