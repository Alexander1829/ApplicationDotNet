using mylabel.MobileId.Aggregator.Db;
using mylabel.MobileId.Aggregator.Jobs.Config;
using mylabel.MobileId.Aggregator.Jobs.JobList.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mylabel.MobileId.Aggregator.Jobs.JobList
{
	public class BillingJob
	{
		ILogger<BillingJob> logger;
		BillingJobSettings settings;

		public BillingJob(ILogger<BillingJob> logger, IOptions<BillingJobSettings> settings)
		{
			this.logger = logger;
			this.settings = settings.Value;
		}

		public async Task CreateAndSendBillingFileAsync()
		{
			var billingQueue = await AggregatorContext.QueryAsync(ctx => ctx.BillingTransactions.ToArrayAsync());

			if (billingQueue.Length > 0)
			{
				var filesCount = CountRemoteFiles();

				var newFileName = NewFileName(filesCount);
				var newFileContent = NewFileContent(billingQueue, newFileName);
				var assemblyDir = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent!.FullName;
				var localFilePath = Path.Combine(assemblyDir, newFileName);

				CreateLocalFile(localFilePath, newFileContent);

				using (var sftp = new SftpClient(settings.Sftp!.Host!, settings.Sftp.Port!.Value, settings.Sftp.UserName!, settings.Sftp.Password))
				{
					sftp.Connect();
					var remoteFilePath = settings!.Sftp!.SavingFolder! + newFileName;
					using (var fs = System.IO.File.OpenRead(localFilePath))
					{
						sftp.UploadFile(fs, remoteFilePath, true);
					}
				}

				await UpdateAuthorizeRequests(billingQueue);

				DeleteLocalFile(localFilePath);

				logger.LogInformation($"Success run: {nameof(BillingJob)}.{nameof(CreateAndSendBillingFileAsync)}."
					+ $"File {newFileName} was sended to Sftp-server {settings.Sftp.Host}");
			}
			logger.LogInformation($"Success empty run: {nameof(BillingJob)}.{nameof(CreateAndSendBillingFileAsync)}");
		}

		object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName)!.GetValue(src, null)!;
		}

		string NewFileName(int filesCount)
		{
			return "MBID_" + DateTimeOffset.Now.ToString("yyyyMMddHHmmss")
				+ "_"
				+ (filesCount + 1).ToString()
				+ "_Moscow_RUS";
		}

		string NewFileContent(BillingTransaction[] listForFile, string fileName)
		{
			var billingProperties = typeof(BillingTransactionFileRow).GetProperties()
				.Where(prop => !Attribute.IsDefined(prop, typeof(NotMappedAttribute))).ToArray();

			StringBuilder fileContent = new StringBuilder();
			for (int i = 0; i < billingProperties.Length; i++)
			{
				if (i == 0)
					fileContent.Append(billingProperties[i].Name);
				else
					fileContent.Append($"|{ billingProperties[i].Name}");
			}
			fileContent.AppendLine();
			for (int rowNum = 0; rowNum < listForFile.Length; rowNum++)
			{
				var transaction = new BillingTransactionFileRow(listForFile[rowNum]);
				transaction.file_name = fileName;
				transaction.us_seq_no = rowNum + 1;

				for (int i = 0; i < billingProperties.Length; i++)
				{
					if (i == 0)
						fileContent.Append(GetPropValue(transaction, billingProperties[i].Name));
					else
						fileContent.Append($"|{ GetPropValue(transaction, billingProperties[i].Name)}");
				}
				fileContent.AppendLine();
			}
			return fileContent.ToString();
		}

		int CountRemoteFiles()
		{
			using (var sftp = new SftpClient(settings!.Sftp!.Host!, settings.Sftp.UserName!, settings.Sftp.Password))
			{
				int filesCount = 0;

				sftp.Connect();
				var files = sftp.ListDirectory(settings!.Sftp!.SavingFolder);

				foreach (var file in files)
				{
					if (!file.Name.StartsWith("."))
						filesCount++;
				}
				return filesCount;
			}
		}

		async Task UpdateAuthorizeRequests(BillingTransaction[] billingQueue)
		{
			var siAuthRequestIds = billingQueue.Where(l => l.SiOrDi == "SI").Select(l => l.AuthorizationId);
			var diAuthRequestIds = billingQueue.Where(l => l.SiOrDi == "DI").Select(l => l.AuthorizationId);

			await AggregatorContext.SaveAsync(ctx =>
			{
				var siAuthRequest = ctx.SIAuthorizationRequests!.Where(req => siAuthRequestIds.Contains(req.Id)).ToList();
				siAuthRequest.ForEach(a => a.BillingSuccess = true);
				var diAuthRequest = ctx.DIAuthorizationRequests!.Where(req => diAuthRequestIds.Contains(req.Id)).ToList();
				diAuthRequest.ForEach(a => a.BillingSuccess = true);

				ctx.BillingTransactions!.RemoveRange(billingQueue);
			});
		}

		void CreateLocalFile(string path, string content)
		{
			using (var fs = new FileStream(path, FileMode.CreateNew))
			using (var writer = new StreamWriter(fs))
			{
				writer.WriteLine(content);
			}
		}

		void DeleteLocalFile(string path)
		{
			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);
		}
	}
}
