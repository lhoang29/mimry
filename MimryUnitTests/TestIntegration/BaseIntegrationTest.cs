using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry;
using SpecsFor.Mvc;

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

            config.UseBrowser(BrowserDriver.Chrome);

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
    }
}
