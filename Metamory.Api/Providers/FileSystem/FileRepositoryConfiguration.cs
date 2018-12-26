

namespace Metamory.Api.Providers.FileSystem
{
	public class FileRepositoryConfiguration : IFileContentRepositoryConfiguration, IFileStatusRepositoryConfiguration
	{
		public string RootPath { get; set; }
		
		string IFileContentRepositoryConfiguration.ContentRootPath => RootPath;

		string IFileStatusRepositoryConfiguration.StatusRootPath => RootPath;
	}
}