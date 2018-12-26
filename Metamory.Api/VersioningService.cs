using System;
using System.Collections.Generic;
using System.Linq;
using Metamory.Api.Repositories;

namespace Metamory.Api
{
	public class VersioningService
	{
		public string GetCurrentlyPublishedVersion(DateTimeOffset now, IEnumerable<ContentStatusEntity> statusEntries)
		{
			var statusEntriesBeforeNow = from entry in statusEntries
										 where entry.StartTime <= now
										 orderby entry.Timestamp
										 select entry;

			string versionId = null;
			foreach (var entry in statusEntriesBeforeNow)
			{
				if (entry.VersionId == versionId)
				{
					versionId = null;
				}
				if (entry.Status == "Published")
				{
					versionId = entry.VersionId;
				}
			}

			return versionId;
		}

		public Dictionary<string, string> GetCurrentStatuses(DateTimeOffset now, IEnumerable<ContentStatusEntity> statusEntries)
		{
			var statusEntriesBeforeNow = from entry in statusEntries
										 where entry.StartTime <= now
										 orderby entry.Timestamp
										 select entry;

			Dictionary<string, string> currentStatuses = new Dictionary<string, string>();
			foreach (var entry in statusEntriesBeforeNow)
			{
				currentStatuses[entry.VersionId] = entry.Status;
			}

			return currentStatuses;
		}
	}
}
