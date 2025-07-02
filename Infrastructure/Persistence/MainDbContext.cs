using AspNetControllerApiBoilerPlate.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetControllerApiBoilerPlate.Infrastructure.Persistence;

public class MainDbContext(DbContextOptions<MainDbContext> options)
    : IdentityDbContext<User>(options)
{
}