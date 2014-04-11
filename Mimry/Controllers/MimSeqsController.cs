using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mimry.Models;
using Mimry.Helpers;

namespace Mimry.Controllers
{
    [Authorize]
    public class MimSeqsController : Controller
    {
        private MimDBContext db = new MimDBContext();

        // GET: /MimSeqs/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.MimSeqs.ToList());
        }

        // GET: /MimSeqs/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = db.MimSeqs.Find(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            return View(mimseq);
        }

        // GET: /MimSeqs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /MimSeqs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MimID,Title,CaptionTop,CaptionBottom")] Mim mim, string imageUrl, string mimryTitle)
        {
            MimSeq mimseq = new MimSeq();
            mimseq.Title = mimryTitle;
            mimseq.CreatedDate = DateTime.Now;

            mim.CreatedDate = DateTime.Now;
            mim.Creator = User.Identity.GetUserId();
            mim.MimSeq = mimseq;
            this.ValidateAddImage(mim, imageUrl);

            if (ModelState.IsValid)
            {
                db.MimSeqs.Add(mimseq);
                db.Mims.Add(mim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mimseq);
        }

        // GET: /MimSeqs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = db.MimSeqs.Find(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            ViewBag.MimSeqID = mimseq.MimSeqID;
            return View();
        }

        // POST: /MimSeqs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MimID,Title,CaptionTop,CaptionBottom")] Mim mim, string imageUrl, int mimseqID)
        {
            mim.CreatedDate = DateTime.Now;
            mim.Creator = User.Identity.GetUserId();
            mim.MimSeqID = mimseqID;
            this.ValidateAddImage(mim, imageUrl);

            if (ModelState.IsValid)
            {
                db.Mims.Add(mim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MimSeqID = mimseqID;
            return View();
        }

        // GET: /MimSeqs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = db.MimSeqs.Find(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            return View(mimseq);
        }

        // POST: /MimSeqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MimSeq mimseq = db.MimSeqs.Find(id);
            db.MimSeqs.Remove(mimseq);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
