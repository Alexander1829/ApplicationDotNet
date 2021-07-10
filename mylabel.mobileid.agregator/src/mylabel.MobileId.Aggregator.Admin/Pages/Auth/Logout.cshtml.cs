using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Admin.Model.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BlazorCookieAuth.Server.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly ILogger logger;
		public LogoutModel(ILogger<LogoutModel> logger)
		{
			this.logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
        {
			// Clear the existing external cookie
			await HttpContext
                .SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

			var message = HttpContext.CreateLogMessage("Logout successfully");

			logger.LogInformation(message);

			return LocalRedirect(Url.Content("~/"));
        }
    }
}