using System;
using System.IO;
using System.Threading.Tasks;
using Metamory.Api;
using Metamory.Api.Providers.FileSystem;

namespace Metamory.ConsoleApp
{
	class Program
	{
		const string SITE_ID = "Test site";
		private static ContentVersioningService _contentManagementService;// = new ContentVersioningService(statusRepository, contentRepository, new VersioningService(), new CanonicalizeService());


		static async Task Main(string[] args)
		{
			var configuration = new FileRepositoryConfiguration { RootPath = "temp" };
			var statusRepository = new FileStatusRepository(configuration);
			var contentRepository = new FileContentRepository(configuration);

			_contentManagementService = new ContentVersioningService(statusRepository, contentRepository, new VersioningService(), new CanonicalizeService());

			var contentId = "index2.html";
			// await StoreContent(contentId);
			// await Publish(contentId, "4a2ebcbf-df9d-42f4-b6ef-40a52369bf15");

			await GetCurrentlyPublishedContent(contentId);
		}

		private static async Task StoreContent(string contentId)
		{
			var contentType = "plain/text";
			var author = "Arjan Einbu";
			var label = "index.html";
			using (var stream = new MemoryStream())
			using (var sw = new StreamWriter(stream))
			{
				sw.WriteLine("<!DOCTYPE html><html><body>Hello World!</body></html>");
				sw.Flush();
				await _contentManagementService.StoreAsync(SITE_ID, contentId, DateTimeOffset.Now, stream, contentType, null, author, label);
			}
		}

		private static async Task Publish(string contentId, string versionId)
		{
			await _contentManagementService.ChangeStatusForContentAsync(SITE_ID, contentId, versionId, "Published", "Arjan Einbu", DateTimeOffset.Now, null);
		}

		private static async Task GetCurrentlyPublishedContent(string contentId)
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					await _contentManagementService.DownloadPublishedContentToStreamAsync(SITE_ID, contentId, DateTimeOffset.Now, stream);

					stream.Seek(0, SeekOrigin.Begin);
					using (var sr = new StreamReader(stream))
					{
						Console.WriteLine(await sr.ReadToEndAsync());
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}


	}
}
