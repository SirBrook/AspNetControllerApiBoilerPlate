using AspNetControllerApiBoilerPlate.Data;
using Microsoft.EntityFrameworkCore;

namespace AspNetControllerApiBoilerPlate.Composition;

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