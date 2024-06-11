using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IPLogsFilter.Db
{
    //public class IPLogsFilterContextFactory : IDesignTimeDbContextFactory<IPLogsFilterContext>
    //{
    //    public IPLogsFilterContext CreateDbContext(string[] args)
    //    {
    //        var builder = new ConfigurationBuilder();
    //        builder.SetBasePath(Directory.GetCurrentDirectory())
    //               .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
    //        IConfiguration config = builder.Build();

    //        var optionsBuilder = new DbContextOptionsBuilder<IPLogsFilterContext>();
    //        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"))
    //                      .UseSnakeCaseNamingConvention();

    //        return new IPLogsFilterContext(optionsBuilder.Options);
    //    }
    //}
}
