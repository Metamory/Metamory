using System.IO;

namespace Metamory.WebApi.Models.WebApi.Content
{
	public class PostContentModel
	{
		public Stream ContentStream { get; set; }
		public string ContentType { get; set; }
		public string PreviousVersionId { get; set; }
		public string Author { get; set; }
		public string Label { get; set; }
	}
}
