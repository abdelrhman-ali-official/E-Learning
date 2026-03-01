
using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Services.Abstractions;
using Services;
using Shared.SecurityModels;
using AutoMapper;
using Persistence.Repositories;
using Services.BackgroundServices;
using FluentValidation;
using FluentValidation.AspNetCore;
using BitaryProject.Extensions;

namespace Start
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Enforce TLS 1.2/1.3 for all outbound connections (e.g. Cloudflare R2)
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls13;

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPresentationServices();
            
            // Add FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Services.AssemblyReference>();
            
            builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<User>>();
            
            builder.Services.ConfigureJwt(builder.Configuration);
            
            // Add DbContext
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });
            
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            builder.Services.AddScoped<IUnitOFWork, UnitOfWork>();
            
            // Add DbInitializer
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            
            builder.Services.Configure<DomainSettings>(builder.Configuration.GetSection("DomainUrls"));
            
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

            builder.Services.Configure<Shared.JaaS.JaaSOptions>(builder.Configuration.GetSection("JaaS"));
            
            builder.Services.AddMemoryCache();
            
            builder.Services.AddHostedService<PurchaseExpirationBackgroundService>();
            builder.Services.AddHostedService<SubscriptionExpirationBackgroundService>();
            builder.Services.AddHostedService<Start.BackgroundServices.LiveSessionExpirationBackgroundService>();

            var app = builder.Build();
            await InitializeDbAsync(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<Start.Middlewares.VideoRateLimitingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            async Task InitializeDbAsync(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeAsync();
            }
        }
    }
}
