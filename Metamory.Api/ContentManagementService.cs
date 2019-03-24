using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Metamory.Api.Repositories;

namespace Metamory.Api
{
	public class ContentManagementService : IDisposable
	{
		private readonly IStatusRepository _statusRepository;
		private readonly IContentRepository _contentRepository;
		private readonly VersioningService _versioningService;
		private readonly CanonicalizeService _canonicalizeService;

		public ContentManagementService(IStatusRepository statusRepository,
			IContentRepository contentRepository,
			VersioningService versioningService,
			CanonicalizeService canonicalizeService)
		{
			_statusRepository = statusRepository;
			_contentRepository = contentRepository;
			_versioningService = versioningService;
			_canonicalizeService = canonicalizeService;
		}

		public async Task<string> GetCurrentlyPublishedVersionIdAsync(string siteId, string contentId, DateTimeOffset now)
		{
			siteId = _canonicalizeService.Canonicalize(siteId);
			contentId = _canonicalizeService.Canonicalize(contentId);

			var statusEntries = await _statusRepository.GetStatusEntriesAsync(siteId, contentId);
			var publishedVersionId = _versioningService.GetCurrentlyPublishedVersion(now, statusEntries);
			return publishedVersionId;
		}

		public async Task<string> DownloadPublishedContentToStreamAsync(string siteId, string contentId, DateTimeOffset now, Stream target)
		{
			var publishedVersionId = await GetCurrentlyPublishedVersionIdAsync(siteId, contentId, now);
			if (publishedVersionId == null) return null;

			var contentType = await DownloadContentToStreamAsync(siteId, contentId, publishedVersionId, target);
			return contentType;
		}

		public async Task ChangeStatusForContentAsync(string siteId, string contentId, string versionId,
			string status, string responsible, DateTimeOffset now, DateTimeOffset? startDate)
		{
			siteId = _canonicalizeService.Canonicalize(siteId);
			contentId = _canonicalizeService.Canonicalize(contentId);
			versionId = _canonicalizeService.Canonicalize(versionId);

			var statusEntry = new ContentStatusEntity
			{
				ContentId = contentId,
				Timestamp = now,
				StartTime = startDate ?? now,
				VersionId = versionId,
				Status = status,
				Responsible = responsible
			};
			await _statusRepository.AddStatusEntryAsync(siteId, statusEntry);
		}

		public async Task<ContentMetadata> GetVersionsAsync(string siteId, string contentId)
		{
			siteId = _canonicalizeService.Canonicalize(siteId);
			contentId = _canonicalizeService.Canonicalize(contentId);

			var versions = await _contentRepository.GetVersionsAsync(siteId, contentId);
			var statusEntries = await _statusRepository.GetStatusEntriesAsync(siteId, contentId);

			var now = DateTimeOffset.Now;
			var publishedVersionId = _versioningService.GetCurrentlyPublishedVersion(now, statusEntries);

			return new ContentMetadata
			{
				Versions = from version in versions
						   orderby version.Timestamp
						   select new ContentMetadata.Version
						   {
							   VersionId = version.VersionId,
							   PreviousVersionId = version.PreviousVersionId,
							   Timestamp = version.Timestamp,
							   Author = version.Author,
							   Label = version.Label,
						   },
				PublishedVersionId = publishedVersionId
			};
		}

		public async Task<ContentMetadata.Version> GetVersionAsync(string siteId, string contentId, string versionId)
		{
			var metadata = await GetVersionsAsync(siteId, contentId);
			return metadata.Versions.SingleOrDefault(x => x.VersionId == versionId);
		}

		public async Task<string> DownloadContentToStreamAsync(string siteId, string contentId, string versionId, Stream target)
		{
			siteId = _canonicalizeService.Canonicalize(siteId);
			contentId = _canonicalizeService.Canonicalize(contentId);
			versionId = _canonicalizeService.Canonicalize(versionId);

			var contentType = await _contentRepository.DownloadContentToStreamAsync(siteId, contentId, versionId, target);
			//TODO: Throw an appropriate exception for File Not Found or File Not Published

			return contentType;
		}

		public async Task<ContentMetadata.Version> StoreAsync(string siteId,
			string contentId,
			DateTimeOffset now,
			Stream contentStream,
			string contentType,
			string previousVersionId = null,
			string author = null,
			string label = null)
		{
			siteId = _canonicalizeService.Canonicalize(siteId);
			contentId = _canonicalizeService.Canonicalize(contentId);
			previousVersionId = _canonicalizeService.Canonicalize(previousVersionId);

			var versionId = Guid.NewGuid().ToString();

			var status = "Draft";
			var statusEntry = new ContentStatusEntity
			{
				ContentId = contentId,
				Timestamp = now,
				StartTime = now,
				VersionId = versionId,
				Status = status
			};
			var t1 = _statusRepository.AddStatusEntryAsync(siteId, statusEntry);

			var t2 = _contentRepository.AddContentAsync(siteId, contentId, versionId, contentStream, contentType, now, previousVersionId, author, label);

			await Task.WhenAll(new[] { t1, t2 });

			return new ContentMetadata.Version
			{
				VersionId = versionId,
				Timestamp = now,
				PreviousVersionId = previousVersionId,
				Author = author,
				Label = label
			};
		}

		//public void DeleteContent(string siteId, string contentId)
		//{
		//	siteId = _canonicalizeService.Canonicalize(siteId);
		//	contentId = _canonicalizeService.Canonicalize(contentId);

		//	throw new NotImplementedException();
		//}

		void IDisposable.Dispose()
		{
			// ...
		}
	}
}