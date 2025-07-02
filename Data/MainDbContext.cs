using AspNetControllerApiBoilerPlate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetControllerApiBoilerPlate.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options)
    : IdentityDbContext<User>(options)
{
}