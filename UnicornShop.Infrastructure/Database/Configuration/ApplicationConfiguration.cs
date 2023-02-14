using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace UnicornShop.Infrastructure.Database.Configuration
{
    public static class ApplicationConfiguration
    {
        // Fluent API

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString, Assembly mappingAssembly)
        {
            services.AddDbContext<DatabaseContext>(opt =>
            {
                opt.UseSqlite(connectionString, config =>
                {
                    config.MigrationsAssembly(mappingAssembly.GetName().Name);
                    config.MigrationsHistoryTable("migration_history", "dbo");
                });

                opt.EnableDetailedErrors(true);
                opt.ConfigureWarnings(e =>
                {
                    e.Default(WarningBehavior.Log);
                });
            });

            return services;
        }

        public static IApplicationBuilder UseMigrations(this IApplicationBuilder app)
        {
            var database = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();

            database.Database.Migrate();

            return app;
        }
    }
}

