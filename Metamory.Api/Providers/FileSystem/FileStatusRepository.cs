using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Metamory.Api.Repositories;

namespace Metamory.Api.Providers.FileSystem
{
	public interface IFileStatusRepositoryConfiguration
	{
		string StatusRootPath { get; }
	}


	public class FileStatusRepository : IStatusRepository
	{
		private const string CONTENTSTATUS_FILENAME = "StatusEntries.csv";
		private readonly IFileStatusRepositoryConfiguration _configuration;

		public FileStatusRepository(IFileStatusRepositoryConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<IEnumerable<ContentStatusEntity>> GetStatusEntriesAsync(string siteId, string contentId)
		{
			var filePath = Path.Combine(_configuration.StatusRootPath, siteId, contentId, CONTENTSTATUS_FILENAME);
			using (var sr = new StreamReader(filePath))
			{
				var everything = await sr.ReadToEndAsync();
				return everything.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
								 .Where(line => !String.IsNullOrWhiteSpace(line))
								 .Select(line => ContentStatusEntity.FromString(line));
			}
		}

		public async Task AddStatusEntryAsync(string siteId, ContentStatusEntity statusEntry)
		{
			var folderPath = Path.Combine(_configuration.StatusRootPath, siteId, statusEntry.ContentId);
			Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, statusEntry.ContentId, CONTENTSTATUS_FILENAME);
			using (var sw = new StreamWriter(filePath, true))
			{
				await sw.WriteLineAsync(statusEntry.ToString());
			}
		}
	}
}