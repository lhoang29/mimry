using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor;
using SpecsFor.Mvc;
using Mimry.Controllers;
using Mimry.Models;
using OpenQA.Selenium;

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
        
        [TestMethod]
        public void MimSeqCreate_WithSession_NoRedirect()
        {
            string returnUrl = String.Empty;
            MimSeqIntegrationTest.Login();
            App.NavigateTo<MimSeqsController>(c => c.Create(returnUrl));
            BaseIntegrationTest.TestRouteMatch(App.Route, "MimSeqs", "Create");
        }

        [TestMethod]
        public void MimSeqEdit_WithoutSession_LinkNotExists()
        {
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
            MimSeqIntegrationTest.Login();
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
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var addElement = App.Browser.FindElement(By.ClassName(MVCConstants.MimryAddLinkClass));
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

        [TestMethod]
        public void MimSeqAdd_Box_WithoutSession_RedirectsToLogin()
        {
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var addElement = App.Browser.FindElement(By.ClassName(MVCConstants.MimryAddBoxClass));
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

        [TestMethod]
        public void MimSeqAdd_Link_WithSession_NoRedirect()
        {
            MimSeqIntegrationTest.Login();
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            Exception exception = null;
            try
            {
                var addElement = App.Browser.FindElement(By.ClassName(MVCConstants.MimryAddLinkClass));
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

        private static void Login()
        {
            App.NavigateTo<AccountController>(c => c.Login(String.Empty));
            App.FindFormFor<LoginViewModel>()
                .Field(m => m.UserName).SetValueTo(MVCConstants.ImpersonateUserName)
                .Field(m => m.Password).SetValueTo(MVCConstants.ImpersonatePassword)
                .Submit();
        }
    }
}
