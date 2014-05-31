using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimry.Controllers;
using System.Drawing;
using System.Web.Mvc;
using System.Net;
using Mimry.Models;
using Mimry.ViewModels;
using MimryUnitTests.MockDAL;

namespace MimryUnitTests.Controllers
{
    [TestClass]
    public class MimUnitTest
    {
        [TestMethod]
        public void TestDetails()
        {
            var mockUOW = new MockUnitOfWork();
            var mc = new MimsController(mockUOW);

            var result = mc.Details(null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, HttpStatusCode.OK);

            result = mc.Details(new Guid());
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestMimage()
        {
            var mockUOW = new MockUnitOfWork();
            var mc = new MimsController(mockUOW);

            var result = mc.Mimage(null);
            Assert.AreEqual(result.GetType(), typeof(HttpStatusCodeResult));

            HttpStatusCodeResult httpResult = result as HttpStatusCodeResult;
            Assert.AreNotEqual(httpResult.StatusCode, HttpStatusCode.OK);
        }
        [TestMethod]
        public void TestVote()
        {
            var mockUOW = new MockUnitOfWork();
            var mc = new MimsController(mockUOW);

            var result = mc.Vote(new Guid(), null);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            string vote = "1";
            result = mc.Vote(new Guid(), vote);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestEdit()
        {
            var mockUOW = new MockUnitOfWork();
            var mc = new MimsController(mockUOW);

            var result = mc.Edit(new Guid());
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestEditPost()
        {
            var mockUOW = new MockUnitOfWork();
            var mc = new MimsController(mockUOW);

            MimEdit me = new MimEdit();
            var result = mc.Edit(me);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));

            me.ImageUrl = "test";
            result = mc.Edit(me);
            Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void TestResize()
        {
            // already small, should return same image
            Bitmap bm = new Bitmap(300, 300);
            var rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 300);
            Assert.AreEqual(rbm.Height, 300);

            bm = new Bitmap(300, 400);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 300);
            Assert.AreEqual(rbm.Height, 400);

            // square
            bm = new Bitmap(600, 600);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 400);
            Assert.AreEqual(rbm.Height, 400);

            // keeping aspect ratio
            bm = new Bitmap(800, 600);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 400);
            Assert.AreEqual(rbm.Height, 300);

            bm = new Bitmap(600, 1200);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 200);
            Assert.AreEqual(rbm.Height, 400);

            bm = new Bitmap(100, 800);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 50);
            Assert.AreEqual(rbm.Height, 400);

            bm = new Bitmap(2400, 3200);
            rbm = MimsController.Resize(bm, 400);
            Assert.AreEqual(rbm.Width, 300);
            Assert.AreEqual(rbm.Height, 400);
        }
    }
}
