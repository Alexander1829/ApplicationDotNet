using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Admin.Model.Helper;
using mylabel.MobileId.Aggregator.Cryptography;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace mylabel.MobileId.Aggregator.Admin.Controllers
{
	[Route("auth")]
	public class AuthController : Controller
	{
		ILogger logger;
		public AuthController(ILogger<AuthController> logger)
		{
			this.logger = logger;
		}

		[HttpGet("login")]
		public async Task<IActionResult> OnLoginAsync([FromQuery(Name = "id")] string credential)
		{
			var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(credential));
			var index = decoded.IndexOf(":");
			var userName = decoded[..index];
			var password = decoded[(index + 1)..];

			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				return LocalRedirect("/");

			var passwordHash = HelperHash.HashString(password);

			var adminUser = await AggregatorContext.QueryAsync(ctx => ctx.AdminUsers!.FirstOrDefaultAsync(l => l.Login == userName && l.Password == passwordHash));
			if (adminUser != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, userName),
					new Claim(ClaimTypes.Role, adminUser.Role ?? ""),
				};
				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var authProperties = new AuthenticationProperties
				{
					AllowRefresh = true,
					ExpiresUtc = DateTimeOffset.Now.AddDays(1),
					IsPersistent = true,
				};

				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties); //Вот это применится только после редиректа. https://stackoverflow.com/a/57261798
			}
			return LocalRedirect("/auth/redirect");
		}

		[HttpGet("redirect")]
		public IActionResult AuthRedirect()
		{
			var logMessage = HttpContext.CreateLogMessage("Login successfully");
			logger.LogInformation(logMessage);
			return LocalRedirect("/");
		}
	}
}
