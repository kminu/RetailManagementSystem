using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RMSApi.Constants;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using RMS.Library.DataAccess;
using RMS.Library.Data;

namespace RMSApi.StartupConfig;

public static class DependencyInjectionExtensions
{
    public static void AddCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDataAccess, SqlDataAccess>();
        builder.Services.AddTransient<IInventoryData, InventoryData>();
        builder.Services.AddTransient<IProductData, ProductData>();
        builder.Services.AddTransient<ISaleData, SaleData>();
        builder.Services.AddTransient<IUserData, UserData>();
    }
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

            // Anyone who has title can access API as long as endpoints do not have specific permission
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
            opts.AddSecurityDefinition("bearerAuth", securityScheme);
            opts.AddSecurityRequirement(securityRequirement);


            // Configure API description
            var title = "RMS API";
            var description = "Retail management system API";
            var terms = new Uri("https://www.Google.com");
            var license = new OpenApiLicense()
            {
                Name = "GitHub MIT License"
            };
            var contact = new OpenApiContact()
            {
                Name = "Google Help Desk",
                Email = "google@google.com",
                Url = new Uri("https://www.Google.com")
            };

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = $"{title} v1 (deprecated)",
                Description = description,
                TermsOfService = terms,
                License = license,
                Contact = contact
            });

            opts.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = $"{title} v2 ",
                Description = description,
                TermsOfService = terms,
                License = license,
                Contact = contact
            });

            // To enable xml comments in swagger, the item below needs to be added in csproj file
            // <GenerateDocumentationFile>true</GenerateDocumentationFile> in <PropertyGroup>
            // to get in csproj, double click project
            var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlfile));
        });

        // API versioning service
        builder.Services.AddApiVersioning(opts =>
        {
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.DefaultApiVersion = new(2, 0);
            opts.ReportApiVersions = true;
        });
        builder.Services.AddVersionedApiExplorer(opts =>
        {
            opts.GroupNameFormat = "'v'VVV";
            opts.SubstituteApiVersionInUrl = true;
        });
    }
    public static void AddHealthCheckServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddSqlServer(builder.Configuration.GetConnectionString("RMSDatabase"));
    }
}