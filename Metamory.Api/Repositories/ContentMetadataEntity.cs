using System;

namespace Metamory.Api.Repositories
{
	public class ContentMetadataEntity
	{
		public DateTimeOffset Timestamp { get; set; }
		public string VersionId { get; set; }
		public string PreviousVersionId { get; set; }
		public string Label { get; set; }
		public string ContentType { get; internal set; }
		public string Author { get; set; }

		public override string ToString()
		{
			return $"{Timestamp};{VersionId};{PreviousVersionId};{Label};{ContentType};{Author}";
		}

		public static ContentMetadataEntity FromString(string line)
		{
			var parts = line.Split(';');
			if (parts.Length != 6) throw new ArgumentException();

			return new ContentMetadataEntity
			{
				Timestamp = DateTimeOffset.Parse(parts[0]),
				VersionId = parts[1],
				PreviousVersionId = parts[2],
				Label = parts[3],
				ContentType = parts[4],
				Author = parts[5]
			};
		}
	}
}