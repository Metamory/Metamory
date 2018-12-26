using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metamory.Api.Repositories
{
	public interface IStatusRepository
	{
		// [Obsolete("Use async version instead")]
		// IEnumerable<ContentStatusEntity> GetStatusEntries(string siteId, string contentId);

		// [Obsolete("Use async version instead")]
		// void AddStatusEntry(string siteId, ContentStatusEntity statusEntry);

		Task<IEnumerable<ContentStatusEntity>> GetStatusEntriesAsync(string siteId, string contentId);
	
		Task AddStatusEntryAsync(string siteId, ContentStatusEntity statusEntry);
	}
}