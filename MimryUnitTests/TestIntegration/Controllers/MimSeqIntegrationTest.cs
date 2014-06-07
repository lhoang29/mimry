using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry.Controllers;
using OpenQA.Selenium;
using System;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimSeqIntegrationTest : BaseIntegrationTest
    {
        [TestMethod]
        public void MimSeqCreate_WithoutSession_RedirectsToLogin()
        {
            BaseIntegrationTest.Logoff();
            string returnUrl = String.Empty;
            App.NavigateTo<MimSeqsController>(c => c.Create(returnUrl));
            BaseIntegrationTest.TestRouteMatch(App.Route, "Account", "Login");
        }

        [TestMethod]
        public void MimSeqCreate_WithSession_NoRedirect()
        {
            string returnUrl = String.Empty;
            BaseIntegrationTest.Login();
            App.NavigateTo<MimSeqsController>(c => c.Create(returnUrl));
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "Create");
        }

        [TestMethod]
        public void MimSeqIndex_WithoutSession_NoRedirect()
        {
            BaseIntegrationTest.Logoff();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "Index");
        }

        [TestMethod]
        public void MimSeqAbout_WithoutSession_NoRedirect()
        {
            BaseIntegrationTest.Logoff();
            App.NavigateTo<MimSeqsController>(c => c.About());
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "About");
        }

        [TestMethod]
        public void MimSeqEdit_WithoutSession_LinkNotExists()
        {
            BaseIntegrationTest.Logoff();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var editElement = App.Browser.FindElement(By.ClassName(MVCConstants.MimryEditClass));
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.AreEqual(exception.GetType(), typeof(NoSuchElementException));
        }

        [TestMethod]
        public void MimSeqEdit_WithSession_LinkExists()
        {
            BaseIntegrationTest.Login();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var editElement = App.Browser.FindElement(By.ClassName(MVCConstants.MimryEditClass));
                Assert.IsNotNull(editElement);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void MimSeqAdd_Link_WithoutSession_RedirectsToLogin()
        {
            MimSeqIntegrationTest.Helper_MimSeqAdd_WithoutSession_RedirectsToLogin(MVCConstants.MimryAddLinkClass);
        }

        [TestMethod]
        public void MimSeqAdd_Box_WithoutSession_RedirectsToLogin()
        {
            MimSeqIntegrationTest.Helper_MimSeqAdd_WithoutSession_RedirectsToLogin(MVCConstants.MimryAddBoxClass);
        }

        [TestMethod]
        public void MimSeqAdd_Link_WithSession_NoRedirect()
        {
            MimSeqIntegrationTest.Helper_MimSeqAdd_WithSession_NoRedirect(MVCConstants.MimryAddLinkClass);
        }

        [TestMethod]
        public void MimSeqAdd_Box_WithSession_NoRedirect()
        {
            MimSeqIntegrationTest.Helper_MimSeqAdd_WithSession_NoRedirect(MVCConstants.MimryAddBoxClass);
        }

        /// <summary>
        /// Navigates to home page, find the add link and navigate to it. Make sure a redirect
        /// to login page was returned.
        /// </summary>
        /// <param name="linkCssClass">The css class of the Add button or anchor element.</param>
        private static void Helper_MimSeqAdd_WithoutSession_RedirectsToLogin(string linkCssClass)
        {
            BaseIntegrationTest.Logoff();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var addElement = App.Browser.FindElement(By.ClassName(linkCssClass));
                Assert.IsNotNull(addElement);
                var addHref = addElement.GetAttribute("href");
                App.Browser.Navigate().GoToUrl(addHref);
                BaseIntegrationTest.TestRouteMatch(App.Route, "Account", "Login");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNull(exception);
        }

        /// <summary>
        /// Logs in, find the add link with the specified css class, navigate to it
        /// and make sure there's no redirect.
        /// </summary>
        /// <param name="linkCssClass">The css class of the Add button or anchor element.</param>
        private static void Helper_MimSeqAdd_WithSession_NoRedirect(string linkCssClass)
        {
            BaseIntegrationTest.Login();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var addElement = App.Browser.FindElement(By.ClassName(linkCssClass));
                Assert.IsNotNull(addElement);
                var addHref = addElement.GetAttribute("href");
                App.Browser.Navigate().GoToUrl(addHref);
                Assert.AreEqual(App.Browser.Url, addHref, ignoreCase: true);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNull(exception);
        }
    }
}
