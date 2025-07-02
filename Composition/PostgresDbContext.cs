using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Composition;

internal static class PostgresDbContext
{
    public static IServiceCollection AddPostgresDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MainDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")!));
        return services;
    }
}