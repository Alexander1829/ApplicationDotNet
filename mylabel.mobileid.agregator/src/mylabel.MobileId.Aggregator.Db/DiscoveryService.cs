using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("DiscoveryServices")]
	public class DiscoveryService
	{
		public int Id { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public string? Uri { get; set; }
	}
}