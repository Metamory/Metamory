using System.Security.Principal;

namespace Metamory.WebApi.Policies
{
	public interface IAuthorizationPolicy
	{
		bool AllowGetCurrentPublishedContent(string siteId, string contentId, IPrincipal principal);
		bool AllowChangeContentStatus(string siteId, string contentId, IPrincipal principal);
		bool AllowManageContent(string siteId, string contentId, IPrincipal principal);
	}
}
