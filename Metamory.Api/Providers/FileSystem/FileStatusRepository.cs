using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Metamory.Api.Repositories;
using Microsoft.Extensions.Options;

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


		public FileStatusRepository(IOptions<FileRepositoryConfiguration> configurationAccessor)
		{
			_configuration = configurationAccessor.Value;
		}


		public async Task<IEnumerable<ContentStatusEntity>> GetStatusEntriesAsync(string siteId, string contentId)
		{
			var filePath = Path.Combine(_configuration.StatusRootPath, siteId, contentId, CONTENTSTATUS_FILENAME);
			
			if(!File.Exists(filePath))
			{
				return Enumerable.Empty<ContentStatusEntity>();
			}
			
			using (var sr = new StreamReader(filePath))
			{
				var everything = await sr.ReadToEndAsync();
				return everything.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
								 .Where(line => !String.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
								 .Select(line => ContentStatusEntity.FromString(line));
			}
		}


		public async Task AddStatusEntryAsync(string siteId, ContentStatusEntity statusEntry)
		{
			var folderPath = Path.Combine(_configuration.StatusRootPath, siteId, statusEntry.ContentId);
			Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, CONTENTSTATUS_FILENAME);
			var writeHeader = !File.Exists(filePath);
			using (var sw = new StreamWriter(filePath, true))
			{
				if(writeHeader)
				{
					await sw.WriteLineAsync($"#Timestamp;ContentId;VersionId;StartTime;Status;Responsible");
				}

				await sw.WriteLineAsync(statusEntry.ToString());
			}
		}
	}
}