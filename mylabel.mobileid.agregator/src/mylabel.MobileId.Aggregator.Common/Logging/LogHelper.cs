using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public static class LogHelper
	{
		public static Guid SetAndGetCurrentActivityId()
		{
			if (Trace.CorrelationManager.ActivityId.Equals(Guid.Empty))
				Trace.CorrelationManager.ActivityId = Guid.NewGuid();
			return Trace.CorrelationManager.ActivityId;
		}
	}
}
