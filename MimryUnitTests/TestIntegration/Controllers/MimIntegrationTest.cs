using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry.Controllers;
using OpenQA.Selenium;
using System;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public void MimDetails_WithoutSession_NoRedirect_NoEditDelete()
        {
            BaseIntegrationTest.Logoff();
            string returnUrl = String.Empty;
            App.NavigateTo<MimSeqsController>(c => c.Index(0));

            var mimDetailsElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimDetailsClass));
            Assert.IsNotNull(mimDetailsElement);

            var mimDetailsHref = mimDetailsElement.GetAttribute("href");
            App.Browser.Navigate().GoToUrl(mimDetailsHref);
            Assert.AreEqual(App.Browser.Url, mimDetailsHref, ignoreCase: true);

            var mimEditElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimEditClass));
            Assert.IsNull(mimEditElement);

            var mimDeleteElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimDeleteClass));
            Assert.IsNull(mimDeleteElement);
        }

        [TestMethod]
        public void MimDetails_WithSession_NoRedirect_EditDeletePresence()
        {
            BaseIntegrationTest.Login();
            string returnUrl = String.Empty;
            App.NavigateTo<MimSeqsController>(c => c.Index(0));

            var mimDetailsElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimDetailsClass));
            Assert.IsNotNull(mimDetailsElement);

            var mimDetailsHref = mimDetailsElement.GetAttribute("href");
            App.Browser.Navigate().GoToUrl(mimDetailsHref);
            Assert.AreEqual(App.Browser.Url, mimDetailsHref, ignoreCase: true);

            var mimEditElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimEditClass));
            Assert.IsNull(mimEditElement);

            var mimDeleteElement = BaseIntegrationTest.FindElement(By.ClassName(MVCConstants.MimDeleteClass));
            Assert.IsNull(mimDeleteElement);

            //TODO: Find a way to reliably access the last Mim in a sequence to test presence of edit/delete buttons
        }
    }
}
