using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Server.Models;

public class User : IdentityUser
{
    [Required]
    [MinLength(3), MaxLength(55)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(3), MaxLength(55)]
    public string LastName { get; set; } = string.Empty;
}