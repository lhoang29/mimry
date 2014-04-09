using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mimry.Models;

namespace Mimry.Controllers
{
    public class MimSeqsController : Controller
    {
        private MimDBContext db = new MimDBContext();

        // GET: /MimSeqs/
        public ActionResult Index()
        {
            return View(db.MimSeqs.ToList());
        }

        // GET: /MimSeqs/Details/5
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
        public ActionResult Create([Bind(Include="MimSeqID,Title,CreatedDate")] MimSeq mimseq)
        {
            if (ModelState.IsValid)
            {
                db.MimSeqs.Add(mimseq);
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
            return View(mimseq);
        }

        // POST: /MimSeqs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MimSeqID,Title,CreatedDate")] MimSeq mimseq)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mimseq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mimseq);
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
