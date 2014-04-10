using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mimry.Models;

namespace Mimry.Controllers
{
    public class MimsController : Controller
    {
        private MimDBContext db = new MimDBContext();

        // GET: /Mims/
        public ActionResult Index()
        {
            var mims = db.Mims.Include(m => m.MimSeq);
            return View(mims.ToList());
        }

        // GET: /Mims/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = db.Mims.Find(id);
            if (mim == null)
            {
                return HttpNotFound();
            }
            return View(mim);
        }

        public ActionResult Mimage(int? id, bool caption = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = db.Mims.Find(id);
            if (mim == null)
            {
                return HttpNotFound();
            }

            byte[] imageData = caption ? MimsController.GenerateMeme(mim) : mim.Image;
            return base.File(imageData, "Image/jpeg");
        }

        // GET: /Mims/Create
        public ActionResult Create()
        {
            ViewBag.MimSeqID = new SelectList(db.MimSeqs, "MimSeqID", "Title");
            return View();
        }

        // POST: /Mims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MimID,Title,CaptionTop,CaptionBottom,MimSeqID")] Mim mim, string imageUrl)
        {
            mim.CreatedDate = DateTime.Now;
            mim.Creator = User.Identity.GetUserId();

            this.ValidateAddImage(mim, imageUrl);

            if (ModelState.IsValid)
            {
                db.Mims.Add(mim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MimSeqID = new SelectList(db.MimSeqs, "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // GET: /Mims/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = db.Mims.Find(id);
            if (mim == null)
            {
                return HttpNotFound();
            }
            ViewBag.MimSeqID = new SelectList(db.MimSeqs, "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // POST: /Mims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MimID,Title,CreatedDate,Creator,Image,CaptionTop,CaptionBottom,MimSeqID")] Mim mim, string imageUrl)
        {
            this.ValidateAddImage(mim, imageUrl);
            if (ModelState.IsValid)
            {
                db.Entry(mim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MimSeqID = new SelectList(db.MimSeqs, "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // GET: /Mims/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = db.Mims.Find(id);
            if (mim == null)
            {
                return HttpNotFound();
            }
            return View(mim);
        }

        // POST: /Mims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mim mim = db.Mims.Find(id);
            db.Mims.Remove(mim);
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

        private void ValidateAddImage(Mim mim, string imageUrl)
        {
            byte[] imageData = null;
            string imageLoadError = String.Empty;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    imageData = wc.DownloadData(imageUrl);
                }
                if (imageData == null)
                {
                    imageLoadError = "Invalid image URL";
                }
                else
                {
                    mim.Image = imageData;
                }
            }
            catch (Exception ex)
            {
                imageLoadError = ex.ToString();
            }

            if (ModelState["Image"].Errors != null)
            {
                ModelState["Image"].Errors.Clear();
            }

            if (!String.IsNullOrEmpty(imageLoadError))
            {
                ModelState.AddModelError("Image", imageLoadError);
            }
        }
        private static byte[] GenerateMeme(Mim mim)
        {
            using (var bmp = new Bitmap(new MemoryStream(mim.Image)))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawString(
                        mim.CaptionTop,
                        new Font("Tahoma", 30),
                        Brushes.White,
                        new RectangleF(0, 0, bmp.Width, bmp.Height / 4),
                        new StringFormat() { Alignment = StringAlignment.Center }
                    );

                    g.DrawString(
                        mim.CaptionBottom,
                        new Font("Tahoma", 30),
                        Brushes.White,
                        new RectangleF(0, bmp.Height - bmp.Height / 4, bmp.Width, bmp.Height / 4),
                        new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far }
                    );
                }
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
