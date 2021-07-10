using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mylabel.MobileId.Aggregator.Db
{
	[Table("DIAuthorizationRequests")]
	public class DIAuthorizationRequest
	{
		public int Id { get; set; }

		[Required]
		public string? StateNew { get; set; } //основной! ключ для состояния

		[Required]
		public string? ClientId { get; set; }

		public string? IdgwClientId { get; set; }

		public string? IdgwClientSecret { get; set; }

		public string? ClientName { get; set; }

		[Required]
		public string? RedirectUri { get; set; }

		[Required]
		public string? Scope { get; set; }

		[Required]
		public string? ResponseType { get; set; }

		[Required]
		public string? AcrValues { get; set; }

		[Required]
		public string? Nonce { get; set; }

		[Required]
		public string? Version { get; set; }

		public string? LoginHint { get; set; }

		public string? Msisdn { get; set; }

		public string? Display { get; set; }

		public DateTimeOffset? CreatedAt { get; set; }

		public string? ServingOperator { get; set; }

		public string? Dcid { get; set; } //ключ для состояния, будущий

		public string? Code { get; set; } //ключ для состояния, будущий

		public string? AccessTokenOnAggregatorHash { get; set; } //ключ для состояния, будущий 

		[Required]
		public string? State { get; set; } //возможно, не уникальный. Храним. Как ключ не используем 

		[Required]
		public bool BillingSuccess { get; set; }

		public string? Error { get; set; }

		public string? ErrorDescription { get; set; }

		public DateTimeOffset? ErrorAt { get; set; }
	}
}