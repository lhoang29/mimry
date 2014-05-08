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
using Mimry.ViewModels;
using Mimry.Helpers;
using Mimry.DAL;
using Mimry.Attributes;

namespace Mimry.Controllers
{
    [AjaxAuthorize("/Account/Login/")]
    public class MimsController : Controller
    {
        private IUnitOfWork m_UOW;
        private ApplicationDbContext userdb = new ApplicationDbContext();

        public MimsController() : this(UnitOfWork.Current) { }

        public MimsController(IUnitOfWork uow)
        {
            m_UOW = uow;
        }

        // GET: /Mims/
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.UserDB = userdb;
            var mims = m_UOW.MimRepository.Get(null, null, includeProperties: "MimSeq");
            return View(mims.ToList());
        }

        // GET: /Mims/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = m_UOW.MimRepository.Get(m => m.MimID == id).Single();
            if (mim == null)
            {
                return HttpNotFound();
            }

            MimDetails md = new MimDetails();
            md.MimID = mim.MimID;
            md.MimryTitle = mim.MimSeq.Title;
            md.Title = mim.Title;
            md.CreatedDate = mim.CreatedDate;
            md.LastModifiedDate = mim.LastModifiedDate;
            md.Creator = mim.GetCreatorName(userdb);
            md.IsOwner = String.Compare(User.Identity.GetUserId(), mim.Creator, StringComparison.OrdinalIgnoreCase) == 0;
            md.IsEditable = (mim.NextMimID == 0);

            return View(md);
        }

        [AllowAnonymous]
        public ActionResult Mimage(Guid? id, bool caption = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = m_UOW.MimRepository.Get(m => m.MimID == id).Single();
            if (mim == null)
            {
                return HttpNotFound();
            }

            byte[] imageData = caption ? MimsController.GenerateMeme(mim) : Convert.FromBase64String(mim.Image);
            return base.File(imageData, "Image/jpeg");
        }

        [HttpPost]
        public ActionResult Vote(Guid id, string vote)
        {
            Mim mim = m_UOW.MimRepository.Get(m => m.MimID == id).Single();
            string userName = User.Identity.GetUserName();
            MimVote mv = m_UOW.MimVoteRepository.GetByID(mim.ID, userName);
            if (mv == null)
            {
                mv = new MimVote() { MimID = mim.ID, User = userName, Vote = Convert.ToInt32(vote) };
                m_UOW.MimVoteRepository.Insert(mv);
            }
            else
            {
                mv.Vote = Convert.ToInt32(vote);
                m_UOW.MimVoteRepository.Update(mv);
            }
            m_UOW.Save();
            return PartialView("MimActions", new MimActionsView() { MimID = mim.MimID, Vote = mv.Vote });
        }

        // GET: /Mims/Create
        public ActionResult Create()
        {
            ViewBag.MimSeqID = new SelectList(m_UOW.MimSeqRepository.Get(), "MimSeqID", "Title");
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
                m_UOW.MimRepository.Insert(mim);
                m_UOW.Save();
                return RedirectToAction("Index");
            }

            ViewBag.MimSeqID = new SelectList(m_UOW.MimSeqRepository.Get(), "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // GET: /Mims/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mim mim = m_UOW.MimRepository.Get(m => m.MimID == id).Single();
            if (mim == null)
            {
                return HttpNotFound();
            }
            if (String.Compare(User.Identity.GetUserId(), mim.Creator, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (mim.NextMimID != 0)
            {
                return RedirectToAction("Details", new { id = id });
            }
            ViewBag.MimSeqID = new SelectList(m_UOW.MimSeqRepository.Get(), "MimSeqID", "Title", mim.MimSeqID);
            return View(mim);
        }

        // POST: /Mims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MimID,Title,Creator,CaptionTop,CaptionBottom")] Mim mim, string imageUrl)
        {
            Mim currentMim = m_UOW.MimRepository.Get(m => m.MimID == mim.MimID).Single();
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
                m_UOW.MimRepository.Update(currentMim);
                m_UOW.Save();
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
            Mim mim = m_UOW.MimRepository.GetByID(id);
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
            Mim mim = m_UOW.MimRepository.GetByID(id);
            m_UOW.MimRepository.Delete(mim);
            m_UOW.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_UOW.Dispose();
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
                return Convert.FromBase64String(mim.Image);
            }
            using (var bmp = new Bitmap(new MemoryStream(Convert.FromBase64String(mim.Image))))
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
