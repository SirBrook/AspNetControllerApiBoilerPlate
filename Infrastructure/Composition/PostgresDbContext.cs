using AspNetControllerApiBoilerPlate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspNetControllerApiBoilerPlate.Infrastructure.Composition;

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