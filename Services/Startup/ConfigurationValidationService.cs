using AspNetControllerApiBoilerPlate.Composition;

namespace AspNetControllerApiBoilerPlate.Services.Startup;

public class ConfigurationValidationService(IConfiguration configuration) : IStartupConfigurationValidator
{
    private const string JwtConfPrefix = "Authentication:Jwt";
    private const string SmtpConfPrefix = "Smtp";

    public void ValidateRequiredSettings()
    {
        var missingSettings = new List<string>();

        // Connection string
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("Postgres")))
            missingSettings.Add("ConnectionStrings:Postgres");

        // JWT
        CheckJwtSetting("Authority");
        CheckJwtSetting("Audience");
        CheckJwtSetting("RsaPublicKey");
        CheckJwtSetting("RsaPrivateKey");

        // SMTP
        CheckSmtpSetting("Port");
        CheckSmtpSetting("Host");
        CheckSmtpSetting("EmailSender");

        if (missingSettings.Count <= 0) return;
        var formatted = string.Join(", ", missingSettings);
        throw new InvalidOperationException($"Missing required configuration values: {formatted}");

        void CheckJwtSetting(string key)
        {
            if (string.IsNullOrWhiteSpace(configuration[$"{JwtConfPrefix}:{key}"]))
                missingSettings.Add(key);
        }

        void CheckSmtpSetting(string key)
        {
            if (string.IsNullOrWhiteSpace($"{SmtpConfPrefix}:{key}"))
                missingSettings.Add(key);
        }
    }
}