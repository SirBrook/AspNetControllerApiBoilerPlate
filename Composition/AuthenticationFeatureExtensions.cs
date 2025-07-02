using System.Security.Cryptography;
using AspNetControllerApiBoilerPlate.Data;
using AspNetControllerApiBoilerPlate.Models;
using AspNetControllerApiBoilerPlate.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AspNetControllerApiBoilerPlate.Composition;

internal static class AuthenticationFeatureExtensions
{
    public static IServiceCollection AddAuthenticationFeature(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddDefaultTokenProviders(); // Add SMS, Mail, Authenticator apps providers

        services.AddAuthentication(options =>
            {
                // Tells ASP.NET Core to use JWT to authenticate users by default (instead of cookies)
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                // Specifies how to respond when a user hits an [Authorize] endpoint but is not authenticated:
                // - With JWT: returns 401 Unauthorized (no redirect)
                // - With Cookies: redirects to the login page (e.g., /Account/Login)
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = configuration["Authentication:Jwt:Authority"];
                jwtOptions.Audience = configuration["Authentication:Jwt:Audience"];
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GenerateJwtRsaIssuerSigningKey(configuration),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Authentication:Jwt:Authority"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Authentication:Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out var token))
                        {
                            context.Token = token; // Attach the token to the context
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        });

        services.AddScoped<AuthenticationService>();

        return services;
    }

    private static RsaSecurityKey GenerateJwtRsaIssuerSigningKey(IConfiguration configuration)
    {
        var asymmetricPublicKey = configuration["Authentication:Jwt:RsaPublicKey"];
        if (string.IsNullOrEmpty(asymmetricPublicKey))
            throw new Exception("Required JWT Secret public Key is missing in the appsettings.*.json file !");

        var rsa = RSA.Create();
        rsa.FromXmlString(asymmetricPublicKey);

        return new RsaSecurityKey(rsa);
    }
}