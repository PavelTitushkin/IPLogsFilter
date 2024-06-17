using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using IPLogsFilter.Bussines.ReadLogsHostedService.Services;
using IPLogsFilter.Bussines.Service;
using IPLogsFilter.DataAccess.Repositories;
using IPLogsFilter.Db;
using IPLogsFilterAPI.Models.ModelsConfig;
using Microsoft.EntityFrameworkCore;
using Minio;

namespace IPLogsFilterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var minioConfig = builder.Configuration.GetRequiredSection("AppSettingsMinioAPI").Get<AppSettingsMinioAPI>();

            // Add services to the container.
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

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.Configure<AppSettingsMinio>(builder.Configuration.GetSection("AppSettingsMinio"));
            builder.Services.Configure<WebLogsProviderConfig>(builder.Configuration.GetSection("WebLogsProveder"));


            builder.Services.AddScoped<IIPLogsFilterRepository, IPLogFilterRepository>();
            builder.Services.AddScoped<ILogFilterService, LogFilterService>();
            builder.Services.AddScoped<IWebLogService, WebLogService>();

            builder.Services.AddHttpClient<IWebLogService, WebLogService>();
            //builder.Services.AddHttpClient<IWebLogService, WebLogService>("WebLogsProvider", client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:7180/");
            //});

            builder.Services.AddHostedService<IPLogsBackgroundReader>();
            //builder.Services.AddHostedService<MinioIpLogsBackgroundReader>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
