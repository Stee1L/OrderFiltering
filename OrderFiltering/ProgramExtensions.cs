using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderFiltering.Data;

namespace OrderFiltering;

public static class ProgramExtensions
{
    public static WebApplicationBuilder SetupBuilder(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.SetupServices(builder.Configuration);
        // builder.Services.AddTransient<AppDbContext>(provider =>
        // {
        //     var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //     optionsBuilder.UseNpgsql(builder.Configuration["CurrentDatabaseConnectionString"]);
        //     return new AppDbContext(optionsBuilder.Options);
        // });
        return builder;
    }

    public static WebApplication InitializeMigrations(this WebApplication app)
    {
        var dbContext = app.Services.GetService<AppDbContext>();
        Console.WriteLine("Начинаю миграцию данных");
        dbContext?.Database.Migrate();
        Console.WriteLine("Миграция данных окончена");
        return app;
    }

    public static WebApplication SetupApplication(this WebApplication app)
    {
        app.UseCors("AllowAllOrigins");

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "Items WebApi v1"); });

        var forwardOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardOptions.KnownNetworks.Clear();
        forwardOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(forwardOptions);

        app.UseRouting();
        app.UseCookiePolicy();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // app.UseAuthentication();
        // app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    private static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //.AllowCredentials()
                        ;
                });
        });
        
        services.AddDbContext<AppDbContext>(options =>
        {
            var dbType = configuration["CurrentDatabaseConnectionString"];
            switch (dbType)
            {
                case "PostgreSQL":
                {
                    options.UseNpgsql(configuration.GetConnectionString(dbType));
                    break;
                }
                case "SQLServer":
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    break;
                }
            }
        });
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API QB",
                Version = "v1",
                Description = "Web API QB"
            });
        });
        
        return services;
    }
}