using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Metamory.WebApi.Policies.Authorization
{
	public class ClaimsAuthorizationPolicy : IAuthorizationPolicy
	{
		public class Entry
		{
			public Regex SiteId { get; }
			public Regex ContentId { get; }
			public bool MustBeAuthenticated { get; }
			public IEnumerable<ClaimRule> ClaimRules { get; }

			public Entry(string siteId, string contentId, bool mustBeAuthenticated, IEnumerable<ClaimRule> claimRules)
			{
				SiteId = new Regex(siteId);
				ContentId = new Regex(contentId);
				MustBeAuthenticated = mustBeAuthenticated;
				ClaimRules = claimRules;
			}
		}

		public class ClaimRule
		{
			public string ClaimName { get; }
			public Regex Value { get; }

			public ClaimRule(string claimname, string value)
			{
				ClaimName = claimname;
				Value = new Regex(value);
			}
		}

		private readonly List<Entry> _allowGetCurrentPublishedContentEntries = new List<Entry>();
		private readonly List<Entry> _allowChangeContentStatusEntries = new List<Entry>();
		private readonly List<Entry> _allowManageContentEntries = new List<Entry>();

		public ClaimsAuthorizationPolicy(XElement authorizationPolicyElement)
		{
			foreach (var siteElement in authorizationPolicyElement.Elements("site"))
			{
				var siteId = (string)siteElement.Attribute("id");
				foreach (var contentElement in siteElement.Elements("content"))
				{
					var contentId = (string)contentElement.Attribute("id");

					_allowGetCurrentPublishedContentEntries.Add(GetEntry(contentElement.Element("allowGetCurrentPublishedContent"), siteId, contentId));
					_allowChangeContentStatusEntries.Add(GetEntry(contentElement.Element("allowChangeContentStatus"), siteId, contentId));
					_allowManageContentEntries.Add(GetEntry(contentElement.Element("allowManageContent"), siteId, contentId));
				}
			}
		}

		private Entry GetEntry(XElement element, string siteId, string contentId)
		{
			var rules =
				element.Elements("claim")
					.Select(
						claimElement => new ClaimRule((string)claimElement.Attribute("name"), (string)claimElement.Attribute("value")));
			return new Entry(siteId, contentId, (bool)element.Attribute("mustBeAuthorized"), rules);
		}

		public bool AllowGetCurrentPublishedContent(string siteId, string contentId, IPrincipal principal)
		{
			return CheckEntries(_allowGetCurrentPublishedContentEntries, siteId, contentId, (ClaimsPrincipal)principal);
		}

		public bool AllowChangeContentStatus(string siteId, string contentId, IPrincipal principal)
		{
			return CheckEntries(_allowChangeContentStatusEntries, siteId, contentId, (ClaimsPrincipal)principal);
		}

		public bool AllowManageContent(string siteId, string contentId, IPrincipal principal)
		{
			return CheckEntries(_allowManageContentEntries, siteId, contentId, (ClaimsPrincipal)principal);
		}

		private static bool CheckEntries(List<Entry> entries, string siteId, string contentId, ClaimsPrincipal claimsPrincipal)
		{
			var relevantEntries = entries.Where(e => e.SiteId.IsMatch(siteId) && e.ContentId.IsMatch(contentId));

		    if (relevantEntries.Any(e => !e.MustBeAuthenticated))
		    {
		        return true;
		    }

		    if (!claimsPrincipal.Identity.IsAuthenticated)
		    {
		        return false;
		    }

            return relevantEntries.Any(
		        e => e.ClaimRules.Count() == 0 || e.ClaimRules.Any(
		            rule => claimsPrincipal.Claims.Any(
		                c => rule.ClaimName == c.Type && rule.Value.IsMatch(c.Value)
		                )
		            )
		        );
		}
	}
}