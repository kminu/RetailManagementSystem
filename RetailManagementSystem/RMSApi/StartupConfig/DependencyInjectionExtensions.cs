using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RMSApi.Constants;
using System.Text;
using Microsoft.OpenApi.Models;

namespace RMSApi.StartupConfig;

public static class DependencyInjectionExtensions
{
    public static void AddStandardServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void AddAuthServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        builder.Configuration.GetValue<string>("Authentication:SecretKey")))
            };
        });
        builder.Services.AddAuthorization(opts =>
        {
            // Define User role 
            opts.AddPolicy(PolicyConstants.Admin, policy =>
            {
                policy.RequireClaim("Title", PolicyConstants.Admin);
            });

            opts.AddPolicy(PolicyConstants.Cashier, policy =>
            {
                policy.RequireClaim("Title", PolicyConstants.Cashier);
            });

            // Anyone who has title can access
            opts.AddPolicy("Title", policy =>
            {
                policy.RequireClaim("Title");
            });

            // Minimum requirement to access API is Authorized user
            opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    public static void AddSwaggerServices(this WebApplicationBuilder builder)
    {
        // Adds Authorize functionality in Swagger
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Description = "JWT Authorization header info using bearer tokens",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        var securityRequirement = new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                new string[]{ }
            }
        };

        builder.Services.AddSwaggerGen(opts =>
        {
            // Activate authorization in swagger
            opts.AddSecurityDefinition("Bearer Authentication", securityScheme);
            opts.AddSecurityRequirement(securityRequirement);


            // Configure API Version
            var title = "Our Versioned API";
            var description = "This is a Web API that demonstrates version.";
            var terms = new Uri("https://www.Google.com");
            var license = new OpenApiLicense()
            {
                Name = "This is my full license information or a link to it."
            };
            var contact = new OpenApiContact()
            {
                Name = "Google Help Desk",
                Email = "google@google.com",
                Url = new Uri("https://www.Google.com")
            };

        });
    }
}