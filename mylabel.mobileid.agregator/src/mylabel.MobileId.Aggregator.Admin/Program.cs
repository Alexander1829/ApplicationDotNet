using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace mylabel.MobileId.Aggregator.Admin
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
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
				});
	}
}
