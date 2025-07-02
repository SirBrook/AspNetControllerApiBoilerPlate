using AspNetControllerApiBoilerPlate.Application.Services.Startup;

namespace AspNetControllerApiBoilerPlate.Infrastructure.Composition;

public static class StartupConfigurationValidatorExtensions
{
    public static IServiceCollection EnsureStartupConfigurationValidation(
        this IServiceCollection services, IConfiguration configuration)
    {
        var validator = new ConfigurationValidationService(configuration);
        validator.ValidateRequiredSettings(); // Executes immediately on startup

        return services;
    }
}