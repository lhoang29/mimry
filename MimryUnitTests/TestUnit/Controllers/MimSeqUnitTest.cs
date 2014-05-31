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
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));
            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreEqual(httpResult.StatusCode, Convert.ToInt32(HttpStatusCode.InternalServerError));
        }
        [TestMethod]
        public void TestDetails()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);
            
            var result = msc.Details(null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, Convert.ToInt32(HttpStatusCode.OK));

            result = msc.Details(new Guid());
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            httpResult = result as HttpStatusCodeResult;
            Assert.AreEqual(httpResult.StatusCode, Convert.ToInt32(HttpStatusCode.InternalServerError));
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
        [TestMethod]
        public void TestCreatePost()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            string imageUrl = "test";
            MimryCreate mc = new MimryCreate() { ImageUrl = imageUrl };
            var result = msc.Create(mc);
            Assert.AreEqual(result.GetType(), typeof(ViewResult));

            Assert.AreEqual(msc.ModelState.IsValid, false);

            var viewResult = result as ViewResult;
            var viewModel = viewResult.ViewData.Model;
            Assert.AreEqual(viewModel.GetType(), typeof(MimryCreate));

            var mimryCreateViewModel = viewModel as MimryCreate;
            Assert.AreEqual(mimryCreateViewModel.ImageUrl, imageUrl);
        }
        [TestMethod]
        public void TestEdit()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            // Test both parameters null
            var result = msc.Edit(null, null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, Convert.ToInt32(HttpStatusCode.OK));

            string returnUrl = "testReturnUrl";
            
            // Test only guid null, make sure same result is returned
            result = msc.Edit(null, returnUrl);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            // Test fake guid and null return url
            result = msc.Edit(new Guid(), null);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            // Test fake guid and dummy return url
            result = msc.Edit(new Guid(), returnUrl);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestLike()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            var result = msc.Like(new Guid());
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestPostComment()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            // Test fake Guid and null comment
            var result = msc.PostComment(new Guid(), null);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            // Test fake Guid and valid comment
            string comment = "This is a test";
            result = msc.PostComment(new Guid(), comment);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestVoteComment()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            // Test fake id and null vote value
            var result = msc.VoteComment(-1, null);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            // Test fake id and valid vote value
            string vote = "1";
            result = msc.VoteComment(0, vote);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestAdd()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            // Test fake guid and null return url value
            var result = msc.Add(null, null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, Convert.ToInt32(HttpStatusCode.OK));

            result = msc.Add(new Guid(), null);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            // Test fake guid and valid return url value
            string returnUrl = "test";
            result = msc.Add(new Guid(), returnUrl);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestAddPost()
        {
            var mockUOW = new MockUnitOfWork();
            var msc = new MimSeqsController(mockUOW);

            string imageUrl = "test";
            MimryContinue mc = new MimryContinue() { ImageUrl = imageUrl };
            var result = msc.Add(mc);
            Assert.AreEqual(result.GetType(), typeof(ViewResult));

            Assert.AreEqual(msc.ModelState.IsValid, false);

            var viewResult = result as ViewResult;
            var viewModel = viewResult.ViewData.Model;
            Assert.AreEqual(viewModel.GetType(), typeof(MimryContinue));

            var mimryContinueViewModel = viewModel as MimryContinue;
            Assert.AreEqual(mimryContinueViewModel.ImageUrl, imageUrl);
        }
    }
}
