namespace mylabel.MobileId.Aggregator.Common
{
	public record Settings
	{
		public string? ServiceDb { get; init; }
		public string? NfsDb { get; init; }
		public string? PrivateKey { get; init; }
		public string? Kid { get; init; }
		public ProxySet? Proxies { get; init; }
		public string? NotificationUri { get; init; }
		public string? DIAuthorizeIdgwCallbackUri { get; init; }
		public string? TokenIssuer { get; init; }
		public PremiumInfoUriSet? PremiumInfoUri { get; init; }
	}

	public record ProxySet
	{
		public Proxy? Default { get; init; }
		public Proxy? Idgw { get; init; }
	}

	public record Proxy
	{
		public string? Address { get; init; }
		public string? UserName { get; init; }
		public string? Password { get; init; }
		public string? Domain { get; init; }
	}

	public record PremiumInfoUriSet
	{ 
		public string? Mts { get; init; }
	}
}