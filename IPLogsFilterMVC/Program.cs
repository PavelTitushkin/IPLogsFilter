using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using IPLogsFilter.Bussines.ReadLogsHostedService.Contracts;
using IPLogsFilter.Bussines.ReadLogsHostedService.Services;
using IPLogsFilter.Bussines.Service;
using IPLogsFilter.DataAccess.Repositories;
using IPLogsFilter.Db;
using IPLogsFilterAPI.Models.ModelsConfig;
using IPLogsFilterMVC.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Minio;

namespace IPLogsFilterMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var minioConfig = builder.Configuration.GetRequiredSection("AppSettingsMinioAPI").Get<AppSettingsMinioAPI>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            if (minioConfig != null)
            {
                builder.Services.AddMinio(configureClient => configureClient
                    .WithEndpoint(minioConfig.EndPoint)
                    .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
                    .WithSSL(false)
                    .Build());
            }

            builder.Services.AddDbContext<IPLogsFilterContext>(
                   optionsBuilder =>
                   {
                       optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                           sqlOptionsBuilder =>
                           {
                               sqlOptionsBuilder.EnableRetryOnFailure();
                           })
                       .UseSnakeCaseNamingConvention();
                   });

            builder.Services.AddAuthentication("LogsScheme")
                .AddScheme<AuthenticationSchemeOptions, LogsAuthenticationMiddleware>("LogsScheme", null);

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("WebMasterOrWebAdmin", policy =>
                    policy.RequireRole("WebMaster", "WebAdmin"));

                options.AddPolicy("ProcessingManagement", policy =>
                    policy.RequireRole("WebAdminSupport")
                          .RequireClaim("Permission", "AccessLogViewer_ProcessingManagement"));

                options.AddPolicy("ProcessingStop", policy =>
                    policy.RequireClaim("Permission", "AccessLogViewer_ProcessingStop"));

                options.AddPolicy("ProcessingStart", policy =>
                    policy.RequireClaim("Permission", "AccessLogViewer_ProcessingStart"));
            });

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.Configure<AppSettingsMinio>(builder.Configuration.GetSection("AppSettingsMinio"));
            builder.Services.Configure<WebLogsProviderConfig>(builder.Configuration.GetSection("WebLogsProveder"));
            
            builder.Services.AddScoped<IWebLogService, WebLogService>();
            builder.Services.AddScoped<ILogFilterService, LogFilterService>();
            builder.Services.AddScoped<IIPLogsFilterRepository, IPLogFilterRepository>();


            builder.Services.AddHttpClient<IWebLogService, WebLogService>();
            //builder.Services.AddHttpClient<IWebLogService, WebLogService>("WebLogsProvider", client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:7180/");
            //});

            builder.Services.AddSingleton<ILogBackgroundReader, IPLogsBackgroundReader>();
            builder.Services.AddHostedService<IPLogsBackgroundReader>();

            //builder.Services.AddSingleton<IPLogsBackgroundReader>();
            //builder.Services.AddHostedService(provider => provider.GetRequiredService<IPLogsBackgroundReader>());
            //builder.Services.AddHostedService<MinioIpLogsBackgroundReader>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();   
            app.UseAuthorization();   

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
