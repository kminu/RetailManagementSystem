using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RMSApi.Constants;
using RMSApi.StartupConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddStandardServices();
builder.AddAuthServices();
builder.AddSwaggerServices();
builder.AddCustomServices();
builder.AddHealthCheckServices();

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("OpenCorsPolicy", opt =>
        opt.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(opts =>
//    {
//        opts.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
//        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//    });
//}

app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    opts.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
