using mylabel.MobileId.Aggregator.BusinessLogic.Dal.Repositories;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.BusinessLogic
{
	public class NfsService
	{
		NfsRepository nfsRepository;

		public NfsService(NfsRepository nfsRepository)
		{
			this.nfsRepository = nfsRepository;
		}

		public Task<string?> GetEmailByMsisdnAsync(string msisdn) => nfsRepository.GetEmailByMsisdnAsync(msisdn);
	}
}