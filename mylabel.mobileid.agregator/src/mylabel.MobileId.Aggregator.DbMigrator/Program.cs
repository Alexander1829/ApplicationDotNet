using mylabel.MobileId.Aggregator.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;

namespace mylabel.MobileId.Aggregator.DbMigrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

			try
			{
				logger.Info("Init main");
				var host = CreateHostBuilder(args).Build();
				using (var scope = host.Services.CreateScope())
					scope.ServiceProvider.GetRequiredService<AggregatorContext>().Database.Migrate();								
				logger.Info("Migrations applied successfully");
				host.Run();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Migrations failed");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
