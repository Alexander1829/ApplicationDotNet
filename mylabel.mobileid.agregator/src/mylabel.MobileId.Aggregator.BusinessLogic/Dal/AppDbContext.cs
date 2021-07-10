using Microsoft.EntityFrameworkCore;

namespace mylabel.MobileId.Aggregator.BusinessLogic.Dal
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
	}
}