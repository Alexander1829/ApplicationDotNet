using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace mylabel.MobileId.Aggregator.Jobs
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Host.CreateDefaultBuilder(args)
				.UseWindowsService()
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.ClearProviders();
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				})
				.UseNLog(new NLogAspNetCoreOptions
				{
					CaptureMessageTemplates = true,
					CaptureMessageProperties = true
				})
				.Build().Run();
		}
	}
}
