using System.Security.Principal;

namespace Metamory.WebApi.Policies.Authorization
{
	public class NoAuthorizationPolicy : IAuthorizationPolicy
	{
		public bool AllowGetCurrentPublishedContent(string siteId, string contentId, IPrincipal principal)
		{
			return true;
		}

		public bool AllowChangeContentStatus(string siteId, string contentId, IPrincipal principal)
		{
			return true;
		}

		public bool AllowManageContent(string siteId, string contentId, IPrincipal principal)
		{
			return true;
		}
	}
}