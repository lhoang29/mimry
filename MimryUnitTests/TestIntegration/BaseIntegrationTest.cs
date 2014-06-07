using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry;
using Mimry.Controllers;
using Mimry.Models;
using OpenQA.Selenium;
using SpecsFor.Mvc;
using System;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class BaseIntegrationTest
    {
        private static MvcWebApp s_App;
        private static SpecsForIntegrationHost s_IntegrationHost;

        protected static MvcWebApp App 
        {
            get { return s_App; }
        }
        
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            var config = new SpecsForMvcConfig();

            config.UseIISExpress()
                .With(Project.Named("Mimry"))
                .ApplyWebConfigTransformForConfig("Release");

            config.BuildRoutesUsing(r => RouteConfig.RegisterRoutes(r));

            config.UseBrowser(BrowserDriver.InternetExplorer);

            s_IntegrationHost = new SpecsForIntegrationHost(config);
            s_IntegrationHost.Start();

            s_App = new MvcWebApp();
        }

        [AssemblyCleanup()]
        public static void Cleanup()
        {
            s_IntegrationHost.Shutdown();
        }

        public static void TestRouteMatch(System.Web.Routing.RouteData route, string expectedController, string expectedAction)
        {
            Assert.IsNotNull(route);
            Assert.AreEqual(expectedController, (string)route.Values[MVCConstants.Controller], true);
            Assert.AreEqual(expectedAction, (string)route.Values[MVCConstants.Action], true);
        }

        public static void Login()
        {
            App.NavigateTo<AccountController>(c => c.Login(String.Empty));
            App.FindFormFor<LoginViewModel>()
                .Field(m => m.UserName).SetValueTo(MVCConstants.ImpersonateUserName)
                .Field(m => m.Password).SetValueTo(MVCConstants.ImpersonatePassword)
                .Submit();
        }

        public static void Logoff()
        {
            App.NavigateTo<MimSeqsController>(c => c.Index(0));
            try
            {
                App.Browser.FindElement(By.Id("logoutForm")).Submit();
            }
            catch (Exception)
            {
                // Exception means not logged in.
            }
        }
    }
}
