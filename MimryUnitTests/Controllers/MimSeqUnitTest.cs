using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry.Controllers;
using System.Web.Mvc;
using System.Net;
using Mimry.Models;
using MimryUnitTests.MockDAL;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimSeqUnitTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);
            
            var result = msc.Details(null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, HttpStatusCode.OK);

            result = msc.Details(new Guid());
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
    }
}
