﻿using System;
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
using Mimry.Helpers;

namespace Mimry.Controllers
{
    [Authorize]
    public class MimsController : Controller
    {
        private MimDBContext db = new MimDBContext();
        private ApplicationDbContext userdb = new ApplicationDbContext();

        // GET: /Mims/
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.UserDB = userdb;
            var mims = db.Mims.Include(m => m.MimSeq);
            return View(mims.ToList());
        }

        // GET: /Mims/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            ViewBag.UserDB = userdb;
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

        [AllowAnonymous]
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
            mim.Creator = User.Identity.GetUserId();

            this.ValidateAddImage(mim, imageUrl, "Image");

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
            if (!User.Identity.IsAuthenticated || !User.Identity.GetUserId().Equals(mim.Creator, StringComparison.OrdinalIgnoreCase))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (mim.CreatedDate < mim.MimSeq.Mims.Max(m => m.CreatedDate))
            {
                return RedirectToAction("Details", id);
            }
            ViewBag.MimSeqID = new SelectList(db.MimSeqs, "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // POST: /Mims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MimID,Title,Creator,CaptionTop,CaptionBottom")] Mim mim, string imageUrl)
        {
            Mim currentMim = db.Mims.Find(mim.MimID);
            if (currentMim == null)
            {
                return HttpNotFound();
            }
            if (!String.IsNullOrWhiteSpace(imageUrl))
            {
                this.ValidateAddImage(mim, imageUrl, "Image");
                currentMim.Image = mim.Image;
            }
            else
            {
                // Empty means to leave value unchanged
                if (ModelState["Image"].Errors != null)
                {
                    ModelState["Image"].Errors.Clear();
                }
            }
            if (ModelState.IsValid)
            {
                currentMim.Title = mim.Title;
                currentMim.CaptionTop = mim.CaptionTop;
                currentMim.CaptionBottom = mim.CaptionBottom;
                db.Entry(currentMim).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = currentMim.MimID });
            }
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

        private static byte[] GenerateMeme(Mim mim)
        {
            if (mim == null)
            {
                return null;
            }
            if (String.IsNullOrWhiteSpace(mim.CaptionTop) && String.IsNullOrWhiteSpace(mim.CaptionBottom))
            {
                return mim.Image;
            }
            using (var bmp = new Bitmap(new MemoryStream(mim.Image)))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    if (!String.IsNullOrWhiteSpace(mim.CaptionTop))
                    {
                        g.DrawString(
                            mim.CaptionTop,
                            new Font("Tahoma", 30),
                            Brushes.White,
                            new RectangleF(0, 0, bmp.Width, bmp.Height / 4),
                            new StringFormat() { Alignment = StringAlignment.Center }
                        );
                    }

                    if (!String.IsNullOrWhiteSpace(mim.CaptionBottom))
                    {
                        g.DrawString(
                            mim.CaptionBottom,
                            new Font("Tahoma", 30),
                            Brushes.White,
                            new RectangleF(0, bmp.Height - bmp.Height / 4, bmp.Width, bmp.Height / 4),
                            new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far }
                        );
                    }
                }
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
