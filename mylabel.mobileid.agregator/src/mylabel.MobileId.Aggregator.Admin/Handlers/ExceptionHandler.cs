using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace mylabel.MobileId.Aggregator.Admin.Handlers
{
	public class ExceptionHandler
	{
		public static void Execute(IApplicationBuilder errorApp)
		{
			errorApp.Run(async context =>
			{	
				var error = context.Features.Get<IExceptionHandlerFeature>();
				if (error != null)
				{
					var ex = error.Error;

					NLog.LogManager.GetCurrentClassLogger().Error(ex);
				}
			});
		}
	}
}
