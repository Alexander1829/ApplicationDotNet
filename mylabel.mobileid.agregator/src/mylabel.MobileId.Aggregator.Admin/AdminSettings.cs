using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Admin
{
	public record AdminSettings
	{
		public string? AggregatorDb { get; init; }
		public string? CsvDelimiter { get; init; }
	}
}
