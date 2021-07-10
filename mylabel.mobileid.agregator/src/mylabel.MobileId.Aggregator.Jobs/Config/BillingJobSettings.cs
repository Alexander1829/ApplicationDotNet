namespace mylabel.MobileId.Aggregator.Jobs.Config
{
	public record BillingJobSettings
	{
		public string CronExpression { get; init; }
		public BillingSftpSet? Sftp { get; init; }
	}
	public record BillingSftpSet
	{
		public string? Host { get; init; }
		public int? Port { get; init; }
		public string? UserName { get; init; }
		public string? Password { get; init; }
		public string? SavingFolder { get; init; }
	}
}
