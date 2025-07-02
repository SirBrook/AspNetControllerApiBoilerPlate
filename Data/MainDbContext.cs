using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options)
    : IdentityDbContext<User>(options)
{
}