using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry.Controllers;
using System.Web.Mvc;
using System.Net;
using Mimry.Models;
using Mimry.ViewModels;
using MimryUnitTests.MockDAL;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimSeqUnitTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            var result = msc.Index();
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
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
        [TestMethod]
        public void TestCreate()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            string returnUrl = "test";
            var result = msc.Create(returnUrl);
            Assert.AreEqual(result.GetType(), typeof(ViewResult));

            var viewResult = result as ViewResult;
            var viewModel = viewResult.ViewData.Model;
            Assert.AreEqual(viewModel.GetType(), typeof(MimryCreate));

            var mimryCreateViewModel = viewModel as MimryCreate;
            Assert.AreEqual(mimryCreateViewModel.ReturnUrl, returnUrl);
        }
    }
}
