using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor;
using SpecsFor.Mvc;
using Mimry.Controllers;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimSeqIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public void MimSeqCreate_WithoutSession_RedirectsToLogin()
        {
            string returnUrl = String.Empty;
            App.NavigateTo<MimSeqsController>(c => c.Create(returnUrl));
            BaseIntegrationTest.TestRouteMatch(App.Route, "Account", "Login");
        }

        [TestMethod]
        public void MimSeqIndex_WithoutSession_NoRedirect()
        {
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "Index");
        }

        [TestMethod]
        public void MimSeqAbout_WithoutSession_NoRedirect()
        {
            App.NavigateTo<MimSeqsController>(c => c.About());
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "About");
        }
    }
}
