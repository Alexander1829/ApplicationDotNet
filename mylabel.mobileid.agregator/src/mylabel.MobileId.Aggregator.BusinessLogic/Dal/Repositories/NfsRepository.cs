using System.Threading.Tasks;
using mylabel.MobileId.Aggregator.Common;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace mylabel.MobileId.Aggregator.BusinessLogic.Dal.Repositories
{
	public class NfsRepository
	{
		Settings settings;

		public NfsRepository(IOptions<Settings> settings)
		{
			this.settings = settings.Value;
		}

		public async Task<string?> GetEmailByMsisdnAsync(string msisdn)
		{
			using var connection = new OracleConnection(settings.NfsDb);

			await connection.OpenAsync();

			using var command = new OracleCommand(@"
				SELECT mail
				FROM xxvip_intf_emp_history_mv
				WHERE nfs_emp_id = (SELECT nfs_emp_id FROM xxvip_intf_phone_mv WHERE regexp_replace(phone, '[^0-9]', '') = :msisdn AND rownum = 1)
				AND rownum = 1
			", connection);

			command.Parameters.Add("msisdn", msisdn);

			return (string?)await command.ExecuteScalarAsync();
		}
	}
}