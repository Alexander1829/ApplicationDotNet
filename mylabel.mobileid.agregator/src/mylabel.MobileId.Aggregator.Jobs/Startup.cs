using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Jobs.Config;
using mylabel.MobileId.Aggregator.Jobs.JobList;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mylabel.MobileId.Aggregator.Jobs
{
    public class Startup
    {
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) => Configuration = configuration;

		public void ConfigureServices(IServiceCollection services)
		{	
			services.AddHangfire(x => x.UseSqlServerStorage(Configuration["ApiApplicationSettings:AggregatorDb"]));
			services.AddHangfireServer();

			services.AddTransient<AggregatorContext>();

			services.Configure<BillingJobSettings>(Configuration.GetSection("BillingJob"));
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

			app.UseHangfireServer();
			app.UseHangfireDashboard();

			RecurringJob.AddOrUpdate<BillingJob>(nameof(BillingJob), x => x.CreateAndSendBillingFileAsync(), Configuration["BillingJob:CronExpression"]);
		}
    }
}
