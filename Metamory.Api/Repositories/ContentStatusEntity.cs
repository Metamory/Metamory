using System;

namespace Metamory.Api.Repositories
{
	public class ContentStatusEntity
	{
		public DateTimeOffset Timestamp { get; set; }   // In Azure table storage, this would be TableEntity.Timestamp
		public string ContentId { get; set; }   // in Azure table storage, this would be TableEntity.PartitionKey
		public string VersionId { get; set; }
		public DateTimeOffset StartTime { get; set; }
		public string Status { get; set; }
		public string Responsible { get; set; }

		public ContentStatusEntity() { }

		public override string ToString()
		{
			return $"{Timestamp:o};{ContentId};{VersionId};{StartTime:o};{Status};{Responsible}";
		}

		public static ContentStatusEntity FromString(string line)
		{
			var parts = line.Split(';');
			if(parts.Length != 6)
				throw new ArgumentException(); //TODO: Throw appropriate exception

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