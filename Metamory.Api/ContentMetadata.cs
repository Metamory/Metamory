using System;

namespace Metamory.Api
{
	public class ContentMetadata
	{
		public string VersionId { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string PreviousVersionId { get; set; }
		public string Author { get; set; }
		public string Label { get; set; }
		public bool IsPublished { get; set; }

		//public string Status { get; set; }
		//public DateTimeOffset StartDate { get; set; }
	}
}