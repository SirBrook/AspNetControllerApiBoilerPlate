using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Entities;

public class User : IdentityUser
{
    [Required]
    [MinLength(3), MaxLength(55)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(3), MaxLength(55)]
    public string LastName { get; set; } = string.Empty;

    // TODO: Find a clean way to add the CreatedAt and UpdatedAt property on entities
}