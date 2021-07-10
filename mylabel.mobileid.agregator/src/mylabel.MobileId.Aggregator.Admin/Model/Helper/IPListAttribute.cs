using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Linq.Enumerable;

namespace mylabel.MobileId.Aggregator.Admin.Model.Helper
{
	public class IPListAttribute : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			var stringValue = value as string;

			if (string.IsNullOrWhiteSpace(stringValue))
				return true;

			foreach (var item in stringValue.Replace(" ", string.Empty).Split(","))
			{
				if (item.Contains("/"))
				{
					var values = item.Split("/");
					if (values.Length != 2)
						return false;
					if (!IPAddress.TryParse(values.First(), out _))
						return false;
					if (!int.TryParse(values.Last(), out var mask) || mask is < 0 or > 32)
						return false;
				}
				else if (item.Contains("-"))
				{
					var borders = item.Split("-");
					if (borders.Length != 2)
						return false;
					if (!IPAddress.TryParse(borders.First(), out var left))
						return false;
					if (!IPAddress.TryParse(borders.Last(), out var right))
						return false;
					if (IPToInteger(left) > IPToInteger(right))
						return false;
				}
				else
				{
					if (!IPAddress.TryParse(item, out _))
						return false;
				}
			}

			return true;
		}

		static uint IPToInteger(IPAddress address)
		{
			var bytes = address.GetAddressBytes();
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
			return BitConverter.ToUInt32(bytes);
		}
	}
}