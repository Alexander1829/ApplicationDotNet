using mylabel.MobileId.Aggregator.Admin.Handlers;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mylabel.MobileId.Aggregator.Admin
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddHttpContextAccessor();

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddAuthentication(
				CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Pages/Auth/Login";
					options.LogoutPath = "/Pages/Auth/Logout";
				});

			services.AddMvc(options => options.EnableEndpointRouting = false);//.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddTransient<AggregatorContext>();

			services.Configure<AdminSettings>(Configuration.GetSection("AdminApplicationSettings"));
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseExceptionHandler(ExceptionHandler.Execute);
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc(); //app.UseMvcWithDefaultRoute();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});

			NLog.LogManager.LoadConfiguration(env.ContentRootPath + "\\nlog.config");
		}
	}
}