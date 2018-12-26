using FakeItEasy;
using Metamory.Api.Repositories;
using NUnit.Framework;

namespace Metamory.Api.Tests
{
	[TestFixture]
	public class ContentVersioningServiceTests
	{
		private ContentVersioningService _contentVersioningSvc;

		[SetUp]
		public void SetUp()
		{
			var statusRepository = A.Fake<IStatusRepository>();
			var contentRepository = A.Fake<IContentRepository>();
			var versioningService = A.Fake<VersioningService>();
			var canonicalizeService = A.Fake<CanonicalizeService>();

			_contentVersioningSvc = new ContentVersioningService(statusRepository, contentRepository, versioningService, canonicalizeService);
		}

		//[Test]
		//public x_x_x()
		//{
		//	var siteId = "testsite";
		//	var contentId = "1";
		//	var versionId = "a";

		//	using (var stream = new MemoryStream())
		//	{
		//		_contentVersioningSvc.DownloadContentToStream(siteId, contentId, versionId, stream);
		//	}
		//}
	}
}
