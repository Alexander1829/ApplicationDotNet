using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace mylabel.MobileId.Aggregator.Db
{
	public class AggregatorContext : DbContext
	{
		public DbSet<ServiceProvider>? ServiceProviders { get; set; }
		public DbSet<SIAuthorizationRequest>? SIAuthorizationRequests { get; set; }
		public DbSet<DIAuthorizationRequest>? DIAuthorizationRequests { get; set; }
		public DbSet<PremiumInfoToken>? PremiumInfoTokens { get; set; }
		public DbSet<AdminUser>? AdminUsers { get; set; }
		public DbSet<BillingTransaction>? BillingTransactions { get; set; }
		public DbSet<DiscoveryService>? DiscoveryServices { get; set; }
		public DbSet<SPRedirectUri>? SPRedirectUris { get; set; }
		public DbSet<SPNotificationUri>? SPNotificationUris { get; set; }
		public DbSet<SPToDiscoveryLink>? SPToDiscoveryLinks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

			var connectionString = root.GetSection("ApiApplicationSettings:AggregatorDb").Value ?? root.GetSection("AdminApplicationSettings:AggregatorDb").Value;

			optionsBuilder.UseSqlServer(connectionString);
		}

		public static async Task<T> QueryAsync<T>(Func<AggregatorContext, Task<T>> workAsync)
		{
			using AggregatorContext context = new();

			context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

			return await workAsync(context);
		}

		public static async Task<int> SaveAsync(Action<AggregatorContext> work)
		{
			using AggregatorContext context = new();

			work(context);

			return await context.SaveChangesAsync();
		}

		public static async Task<int> SaveAsync(Func<AggregatorContext, Task> workAsync)
		{
			using AggregatorContext context = new();

			await workAsync(context);

			return await context.SaveChangesAsync();
		}
	}
}