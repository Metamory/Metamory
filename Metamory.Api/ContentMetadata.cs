using System;
using System.Collections.Generic;

namespace Metamory.Api
{
	public class ContentMetadata
	{
		public class Version
		{
			public string VersionId { get; set; }
			public DateTimeOffset Timestamp { get; set; }
			public string PreviousVersionId { get; set; }
			public string Author { get; set; }
			public string Label { get; set; }
			//public string Status { get; set; }
			//public DateTimeOffset StartDate { get; set; }
		}

		public IEnumerable<Version> Versions { get; set; }
		public string PublishedVersionId { get; set; }
	}
}