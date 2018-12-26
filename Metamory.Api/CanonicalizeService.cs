namespace Metamory.Api
{
	public class CanonicalizeService
	{
		public string Canonicalize(string id)
		{
			//TODO: replace or remove invalid characters in file or directory name
			// Invalid chars: /\:*%
			// Check if I can use url-escaping rules.
			return id;
		}
	}
}
