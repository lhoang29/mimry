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
using Mimry.ViewModels;

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
        public ActionResult Create([Bind(Include = "MimryTitle,MimTitle,ImageUrl,CaptionTop,CaptionBottom")] MimryCreate mc)
        {
            Mim mim = new Mim();
            this.ValidateAddImage(mim, mc.ImageUrl, "ImageUrl");

            if (ModelState.IsValid)
            {
                MimSeq mimseq = new MimSeq();
                mimseq.Title = mc.MimryTitle;

                mim.Creator = User.Identity.GetUserId();
                mim.Title = mc.MimTitle;
                mim.CaptionTop = mc.CaptionTop;
                mim.CaptionBottom = mc.CaptionBottom;
                mim.MimSeq = mimseq;

                db.MimSeqs.Add(mimseq);
                db.Mims.Add(mim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mc);
        }

        public ActionResult Edit(Guid? id)
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
            MimryEdit me = new MimryEdit() { MimSeqID = mimseq.MimSeqID, Title = mimseq.Title };
            return View(me);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MimSeqID,Title")] MimryEdit me)
        {
            if (ModelState.IsValid)
            {
                MimSeq ms = db.MimSeqs.Find(me.MimSeqID);
                if (ms == null)
                {
                    return HttpNotFound();
                }
                ms.Title = me.Title;
                db.Entry(ms).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(me);
        }

        // GET: /MimSeqs/Add/5
        public ActionResult Add(Guid? id)
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
            MimryContinue mc = new MimryContinue() { MimSeqID = mimseq.MimSeqID };
            return View(mc);
        }

        // POST: /MimSeqs/Add/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "MimSeqID,Title,ImageUrl,CaptionTop,CaptionBottom")] MimryContinue mc)
        {
            Mim mim = new Mim();
            this.ValidateAddImage(mim, mc.ImageUrl, "ImageUrl");

            if (ModelState.IsValid)
            {
                mim.Creator = User.Identity.GetUserId();
                mim.MimSeqID = mc.MimSeqID;
                mim.Title = mc.Title;
                mim.CaptionTop = mc.CaptionTop;
                mim.CaptionBottom = mc.CaptionBottom;

                Mim prevMim = db.Mims.Single(m => m.MimSeqID == mim.MimSeqID && m.NextMimID == 0);
                if (prevMim == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                mim.PrevMimID = prevMim.ID;
                db.Mims.Add(mim);
                db.SaveChanges();

                prevMim.NextMimID = mim.ID;
                db.Entry(prevMim).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(mc);
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
