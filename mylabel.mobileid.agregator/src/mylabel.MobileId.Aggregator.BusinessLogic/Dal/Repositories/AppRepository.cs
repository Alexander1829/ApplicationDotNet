namespace mylabel.MobileId.Aggregator.BusinessLogic.Dal.Repositories
{
	public class AppRepository
	{
		readonly AppDbContext _appDbContext;

		public AppRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}
	}
}