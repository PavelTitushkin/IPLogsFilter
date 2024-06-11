using IPLogsFilter.Abstractions.Repositories;
using IPLogsFilter.Abstractions.Services;
using IPLogsFilter.Bussines.ReadLogsHostedService.Configuration;
using IPLogsFilter.Bussines.ReadLogsHostedService.Services;
using IPLogsFilter.Bussines.Service;
using IPLogsFilter.DataAccess.Repositories;
using IPLogsFilter.Db;
using Microsoft.EntityFrameworkCore;

namespace IPLogsFilterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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

            builder.Services.AddScoped<IIPLogsFilterRepository, IPLogFilterRepository>();
            builder.Services.AddScoped<ILogFilterService, LogFilterService>();

            builder.Services.AddHostedService<IPLogsBackgroundReader>();

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
