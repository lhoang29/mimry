using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor;
using SpecsFor.Mvc;
using Mimry.Controllers;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class AccountIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public void AccountManage_WithoutSession_RedirectsToLogin()
        {
            AccountController.ManageMessageId? messageId = null;
            App.NavigateTo<AccountController>(c => c.Manage(messageId));

            BaseIntegrationTest.TestRouteMatch(App.Route, "Account", "Login");
        }
    }
}
