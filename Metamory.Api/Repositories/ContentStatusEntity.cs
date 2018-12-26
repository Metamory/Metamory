using System;

namespace Metamory.Api.Repositories
{
	public class ContentStatusEntity
	{
		public string ContentId { get; set; }   // in Azure table storage, this would be TableEntity.PartitionKey
		public DateTimeOffset Timestamp { get; set; }   // In Azure table storage, this would be TableEntity.Timestamp
		public DateTimeOffset StartTime { get; set; }
		public string Status { get; set; }
		public string VersionId { get; set; }
		public string Responsible { get; set; }

		public ContentStatusEntity() { }

		public override string ToString()
		{
			return $"{Timestamp};{ContentId};{VersionId};{StartTime};{Status};{Responsible}";
		}

		public static ContentStatusEntity FromString(string line)
		{
			var parts = line.Split(';');
			if(parts.Length != 6) throw new ArgumentException();

			return new ContentStatusEntity{
				Timestamp = DateTimeOffset.Parse(parts[0]),
				ContentId = parts[1],
				VersionId = parts[2],
				StartTime = DateTimeOffset.Parse(parts[3]),
				Status = parts[4],
				Responsible = parts[5],
			};
		}
	}
}