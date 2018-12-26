using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Metamory.Api.Repositories
{
	public interface IContentRepository
	{
		// [Obsolete("Use async version instead")]
		// string DownloadContentToStream(string siteId, string contentId, string versionId, Stream target);

		// [Obsolete("Use async version instead")]
		// void AddContent(string siteId, string contentId, string versionId, Stream contentStream, string contentType, DateTimeOffset now, string previousVersionId, string author, string label);

		// [Obsolete("Use async version instead")]
		// IEnumerable<VersionCargo> GetVersions(string siteId, string contentId);

		Task<string> DownloadContentToStreamAsync(string siteId, string contentId, string versionId, Stream memoryStream);
		
		Task AddContentAsync(string siteId, string contentId, string versionId, Stream contentStream, string contentType, DateTimeOffset now, string previousVersionId, string author, string label);
		
		Task<IEnumerable<ContentMetadataEntity>> GetVersionsAsync(string siteId, string contentId);
	}
}