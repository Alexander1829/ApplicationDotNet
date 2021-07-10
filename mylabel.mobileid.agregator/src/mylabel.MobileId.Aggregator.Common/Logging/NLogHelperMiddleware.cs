using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public class NLogHelperMiddleware
	{
		private readonly RequestDelegate _next;

		public NLogHelperMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			LogHelper.SetAndGetCurrentActivityId();

			await _next(context);
		}
	}
}
