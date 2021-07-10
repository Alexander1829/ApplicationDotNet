using System;
using mylabel.MobileId.Aggregator.Api;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

var host = Host
	.CreateDefaultBuilder(args)
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
	.Build();

host.Run();