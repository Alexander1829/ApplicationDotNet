using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("SPRedirectUris")]
	public class SPRedirectUri
	{
		public int Id { get; set; }

		public int ServiceProviderId { get; set; }

		[Required]
		public string? Value { get; set; }
	}
}
