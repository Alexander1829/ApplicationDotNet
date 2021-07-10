using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("AdminUsers")]
	public class AdminUser
	{
		public int Id { get; set; }

		[Required]
		public string? Login { get; set; }

		[Required]
		public string? Password { get; set; }

		[Required]
		[Range(typeof(string), "Admin", "User")]
		public string? Role { get; set; }
	}
}
