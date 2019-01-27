using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Metamory.Api.Repositories;
using Microsoft.Extensions.Options;

namespace Metamory.Api.Providers.FileSystem
{
	public interface IFileContentRepositoryConfiguration
	{
		string ContentRootPath { get; }
	}


	public class FileContentRepository : IContentRepository
	{
		private const string METADATA_EXTENSION = "metadata.json";

		private readonly IFileContentRepositoryConfiguration _configuration;


		public FileContentRepository(IOptions<FileRepositoryConfiguration> configurationAccessor)
		{
			_configuration = configurationAccessor.Value;
		}


		public async Task<string> DownloadContentToStreamAsync(string siteId, string contentId, string versionId, Stream memoryStream)
		{
			var contentFilePath = Path.Combine(_configuration.ContentRootPath, siteId, contentId, versionId);
			var metadataFilePath = Path.ChangeExtension(contentFilePath, METADATA_EXTENSION);

			using (var sr = new StreamReader(metadataFilePath))
			using (var fileStream = File.OpenRead(contentFilePath))
			{
				var line = await sr.ReadLineAsync();
				var metadata = ContentMetadataEntity.FromString(line);

				await fileStream.CopyToAsync(memoryStream);

				return metadata.ContentType;
			}
		}

		public async Task AddContentAsync(string siteId, string contentId, string versionId, Stream contentStream, string contentType, DateTimeOffset now, string previousVersionId, string author, string label)
		{
			var folderPath = Path.Combine(_configuration.ContentRootPath, siteId, contentId);
			Directory.CreateDirectory(folderPath);

			var contentFilePath = Path.Combine(folderPath, versionId);
			var metadataFilePath = Path.ChangeExtension(contentFilePath, METADATA_EXTENSION);

			using (var sw = new StreamWriter(metadataFilePath, false))
			using (var fileStream = File.OpenWrite(contentFilePath))
			{
				var metadata = new ContentMetadataEntity
				{
					Timestamp = now,
					VersionId = versionId,
					PreviousVersionId = previousVersionId,
					Label = label,
					ContentType = contentType,
					Author = author
				};
				await sw.WriteLineAsync(metadata.ToString());

				contentStream.Seek(0, SeekOrigin.Begin);
				await contentStream.CopyToAsync(fileStream);
			}
		}

		public async Task<IEnumerable<ContentMetadataEntity>> GetVersionsAsync(string siteId, string contentId)
		{
			var folderPath = Path.Combine(_configuration.ContentRootPath, siteId, contentId);

			if (!Directory.Exists(folderPath))
			{
				return Enumerable.Empty<ContentMetadataEntity>();
			}

			var metadataTasks = Directory.GetFiles(folderPath, $"*.{METADATA_EXTENSION}")
										 .Select(async filename => await FromFile(filename))
										 .ToArray();

			return await Task.WhenAll(metadataTasks);
		}

		private async Task<ContentMetadataEntity> FromFile(string path)
		{
			using (var sr = new StreamReader(path))
			{
				var s = await sr.ReadLineAsync();
				return ContentMetadataEntity.FromString(s);
			}
		}
	}
}