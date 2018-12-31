using System.Collections.Generic;
using System.Security.Claims;
using System.Xml.Linq;
using FakeItEasy;
using Metamory.WebApi.Policies.Authorization;
using NUnit.Framework;

namespace Metamory.WebApi.Tests.Policies.Authorization
{
	[TestFixture]
	public class ClaimsAuthorizationPolicyTests
	{
	    private ClaimsAuthorizationPolicy _policy;

	    [SetUp]
	    public void Setup()
	    {
            var doc = XDocument.Load("authorization.config");
	        _policy = new ClaimsAuthorizationPolicy(doc.Element("authorizationPolicy"));
	    }

        [Test] // Testing the "must be authenticated and has the correct claim" rule
        public void AllowChangeContentStatus_RegisteredSiteAndDocument_PrincipalHasCorrectClaims_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim> { new Claim("group", "publisher") });
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowChangeContentStatus("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }

        [Test]
        public void AllowChangeContentStatus_RegisteredSiteAndDocument_PrincipalDoesntHaveCorrectClaims_ReturnsFalse()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim> { new Claim("group", "contributor") });
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowChangeContentStatus("site A", "content 1", p);

            Assert.That(allowed, Is.False);
        }

        [Test]
        public void AllowChangeContentStatus_RegisteredSiteAndDocument_PrincipalDoesntHaveAnyClaims_ReturnsFalse()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim>());

            var allowed = _policy.AllowChangeContentStatus("site A", "content 1", p);

            Assert.That(allowed, Is.False);
        }


        [Test] // Testing the "is authenticated and needs no claim" rule
        public void AllowManageContent_MustBeAuthenticated_UserIsAuthenticatedAndHasClaims_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim> { new Claim("group", "publisher") });
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowManageContent("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }

        [Test]
        public void AllowManageContent_MustBeAuthenticated_UserIsAuthenticatedButNoClaims_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim>());
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowManageContent("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }

        [Test]
        public void AllowManageContent_MusteBeAuthenticated_UserIsAnonomous_ReturnsFalse()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim>());

            var allowed = _policy.AllowManageContent("site A", "content 1", p);

            Assert.That(allowed, Is.False);
        }


        [Test] // Testing the "needs not be authenticated" rule
        public void AllowGetCurrentPublishedContent_NeedsNotBeAuthenticated_UserIsAuthenticatedAndHasClaims_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim> { new Claim("group", "publisher") });
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowGetCurrentPublishedContent("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }

        [Test]
        public void AllowGetCurrentPublishedContent_NeedsNotBeAuthenticated_UserIsAuthenticatedButNoClaims_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim>());
            var i = A.Fake<ClaimsIdentity>();
            A.CallTo(() => i.IsAuthenticated).Returns(true);
            A.CallTo(() => p.Identity).Returns(i);

            var allowed = _policy.AllowGetCurrentPublishedContent("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }

        [Test]
        public void AllowGetCurrentPublishedContent_NeedsNotBeAuthenticated_UserIsAnonomous_ReturnsTrue()
        {
            var p = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => p.Claims).Returns(new List<Claim>());

            var allowed = _policy.AllowGetCurrentPublishedContent("site A", "content 1", p);

            Assert.That(allowed, Is.True);
        }


    }
}
