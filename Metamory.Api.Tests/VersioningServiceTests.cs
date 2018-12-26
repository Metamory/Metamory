using System;
using Metamory.Api.Repositories;
using NUnit.Framework;

namespace Metamory.Api.Tests
{
	[TestFixture]
	public class VersioningServiceTests
	{
		private VersioningService _versioningSvc;

		[SetUp]
		public void SetUp()
		{
			_versioningSvc = new VersioningService();
		}

		[Test]
		public void GetCurrentlyPublishedVersion_NoRows_ReturnsNull()
		{
			var dtNow = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var statusEntries = new ContentStatusEntity[0];

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentlyPublishedVersion_OneRowNonePublished_ReturnsNull()
		{
			var dt = new DateTimeOffset(2015, 1,1,7,0,0, TimeSpan.FromHours(-2));
			var dtNow = dt.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt, StartTime = dt, VersionId = "1", Status = "Ready for review"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentlyPublishedVersion_TwoRowsNonePublished_ReturnsNull()
		{
			var dt1 = new DateTimeOffset(2015, 1,1,7,0,0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Ready for review"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "2", Status = "Ready for review"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentlyPublishedVersion_OneRowPublished_ReturnsVersion()
		{
			var dt = new DateTimeOffset(2015, 1,1,7,0,0, TimeSpan.FromHours(-2));
			var dtNow = dt.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt, StartTime = dt, VersionId = "1", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.EqualTo("1"));
		}

		[Test]
		public void GetCurrentlyPublishedVersion_OneRowsPublishedInTheFuture_ReturnsNull()
		{
			var dt = new DateTimeOffset(2015, 1,1,7,0,0, TimeSpan.FromHours(-2));
			var dtNow = dt.AddYears(-1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt, StartTime = dt, VersionId = "1", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentlyPublishedVersion_TwoRowsOnePublished_ReturnsVersion()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Ready for review"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "1", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.EqualTo("1"));
		}

		[Test]
		public void GetCurrentlyPublishedVersion_TwoRowsOnePublishedInTheFuture_ReturnsNull()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddMinutes(30);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Ready for review"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "1", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentlyPublishedVersion_TwoRowsTwoPublished_ReturnsVersion()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Published"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "2", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.EqualTo("2"));
		}

		[Test]
		public void GetCurrentlyPublishedVersion_TwoRowsTwoPublishedOneInTheFuture_ReturnsVersion()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddMinutes(30);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Published"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "2", Status = "Published"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.EqualTo("1"));
		}

		[Test]
		public void GetCurrentlyPublishedVersion_ThreeRowsMiddleOnePublished_ReturnsVersion()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dt3 = dt1.AddHours(2);
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Ready for review"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "1", Status = "Published"},
				new ContentStatusEntity{Timestamp = dt3, StartTime = dt3, VersionId = "2", Status = "Ready for review"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.EqualTo("1"));
		}

		[Test]
		public void GetCurrentlyPublishedVersion_FirstPublishedThenUnpublished_ReturnsNull()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Published"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "1", Status = "Ready for review"}
			};

			var currentVersion = _versioningSvc.GetCurrentlyPublishedVersion(dtNow, statusEntries);

			Assert.That(currentVersion, Is.Null);
		}

		[Test]
		public void GetCurrentStatuses_NoStatusEntries_EmptyDictionary()
		{
			var dtNow = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var statusEntries = new ContentStatusEntity[0];

			var statuses = _versioningSvc.GetCurrentStatuses(dtNow, statusEntries);

			Assert.That(statuses, Is.Empty);
		}

		[Test]
		public void GetCurrentStatuses_OneStatusEntryForOneContent_DictionaryOfOne()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Draft"}
			};

			var statuses = _versioningSvc.GetCurrentStatuses(dtNow, statusEntries);

			Assert.That(statuses.Count, Is.EqualTo(1));
		}

		[Test]
		public void GetCurrentStatuses_SingleStatusEntriesForTwoContents_DictionaryOfTwo()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dtNow = dt1.AddYears(1);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Draft"},
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "2", Status = "Published"}
			};

			var statuses = _versioningSvc.GetCurrentStatuses(dtNow, statusEntries);

			Assert.That(statuses.Count, Is.EqualTo(2));
		}

		[Test]
		public void GetCurrentStatuses_OneFutureStatusEntryForOneContent_DictionaryOfOne()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			//var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddMinutes(-30);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Draft"}
			};

			var statuses = _versioningSvc.GetCurrentStatuses(dtNow, statusEntries);

			Assert.That(statuses, Is.Empty);
		}

		[Test]
		public void GetCurrentStatuses_StatusChangesInTheFuture_DictionaryContainsCurrentStatus()
		{
			var dt1 = new DateTimeOffset(2015, 1, 1, 7, 0, 0, TimeSpan.FromHours(-2));
			var dt2 = dt1.AddHours(1);
			var dtNow = dt1.AddMinutes(30);
			var statusEntries = new[]
			{
				new ContentStatusEntity{Timestamp = dt1, StartTime = dt1, VersionId = "1", Status = "Draft"},
				new ContentStatusEntity{Timestamp = dt2, StartTime = dt2, VersionId = "1", Status = "Published"}
			};

			var statuses = _versioningSvc.GetCurrentStatuses(dtNow, statusEntries);

			Assert.That(statuses.Count, Is.EqualTo(1));
			Assert.That(statuses["1"], Is.EqualTo("Draft"));
		}
	}
}
