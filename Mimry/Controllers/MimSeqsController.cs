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
using Mimry.DAL;

namespace Mimry.Controllers
{
    [Authorize]
    public class MimSeqsController : Controller
    {
        private IUnitOfWork m_UOW;
        private ApplicationDbContext userdb = new ApplicationDbContext();

        public MimSeqsController() : this(UnitOfWork.Current) { }

        public MimSeqsController(IUnitOfWork uow)
        {
            m_UOW = uow;
        }

        // GET: /MimSeqs/
        [AllowAnonymous]
        public ActionResult Index()
        {
            var mimSeqs = m_UOW.MimSeqRepository.Get();
            var mimSeqViews = new List<MimSeqView>();
            foreach (var ms in mimSeqs)
            {
                mimSeqViews.Add(this.ToMimSeqView(ms, MimViewMode.Thumbnail));
            }
            return View(mimSeqViews);
        }

        // GET: /MimSeqs/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            return View(this.ToMimSeqView(mimseq, MimViewMode.Full));
        }

        // GET: /MimSeqs/Create
        public ActionResult Create(string returnUrl)
        {
            MimryCreate mc = new MimryCreate() { ReturnUrl = returnUrl };
            return View(mc);
        }

        // POST: /MimSeqs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MimryTitle,MimTitle,ImageUrl,CaptionTop,CaptionBottom,ReturnUrl")] MimryCreate mc)
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

                m_UOW.MimSeqRepository.Insert(mimseq);
                m_UOW.MimRepository.Insert(mim);
                m_UOW.Save();

                if (!String.IsNullOrWhiteSpace(mc.ReturnUrl))
                {
                    return Redirect(mc.ReturnUrl);
                }
                return RedirectToAction("Index");
            }

            return View(mc);
        }

        public ActionResult Edit(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            if (!IsCurrentUserMimSeqOwner(mimseq))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimryEdit me = new MimryEdit() { MimSeqID = mimseq.MimSeqID, Title = mimseq.Title, ReturnUrl = returnUrl };
            return View(me);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MimSeqID,Title,ReturnUrl")] MimryEdit me)
        {
            if (ModelState.IsValid)
            {
                MimSeq ms = m_UOW.MimSeqRepository.GetByID(me.MimSeqID);
                if (ms == null)
                {
                    return HttpNotFound();
                }
                if (!IsCurrentUserMimSeqOwner(ms))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ms.Title = me.Title;

                m_UOW.MimSeqRepository.Update(ms);
                m_UOW.Save();

                if (!String.IsNullOrWhiteSpace(me.ReturnUrl))
                {
                    return Redirect(me.ReturnUrl);
                }
                return RedirectToAction("Index");
            }
            return View(me);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Like(Guid id)
        {
            if (Request.IsAuthenticated)
            {
                MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
                if (mimseq == null)
                {
                    return HttpNotFound();
                }

                string userName = User.Identity.GetUserName();
                MimSeqLike msl = m_UOW.MimSeqLikeRepository.GetByID(id, userName);
                if (msl == null)
                {
                    msl = new MimSeqLike() { MimSeqID = id, User = userName };
                    m_UOW.MimSeqLikeRepository.Insert(msl);
                }
                else
                {
                    m_UOW.MimSeqLikeRepository.Delete(msl);
                }
                m_UOW.Save();
                return new JsonResult() { Data = "success" };
            }
            else
            {
                return new JsonResult() { Data = Url.Action("Login", "Account") };
            }
        }

        // GET: /MimSeqs/Add/5
        public ActionResult Add(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }
            MimryContinue mc = new MimryContinue() { MimSeqID = mimseq.MimSeqID, ReturnUrl = returnUrl };
            return View(mc);
        }

        // POST: /MimSeqs/Add/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "MimSeqID,Title,ImageUrl,CaptionTop,CaptionBottom,ReturnUrl")] MimryContinue mc)
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

                Mim prevMim = m_UOW.MimRepository.Get(m => m.MimSeqID == mim.MimSeqID && m.NextMimID == 0).Single();
                if (prevMim == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                mim.PrevMimID = prevMim.ID;
                m_UOW.MimRepository.Insert(mim);
                m_UOW.Save();

                prevMim.NextMimID = mim.ID;
                m_UOW.MimRepository.Update(prevMim);
                m_UOW.Save();

                if (!String.IsNullOrWhiteSpace(mc.ReturnUrl))
                {
                    return Redirect(mc.ReturnUrl);
                }
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
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
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
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            m_UOW.MimSeqRepository.Delete(mimseq);
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

        private MimSeqView ToMimSeqView(MimSeq ms, MimViewMode vm)
        {
            var msv = new MimSeqView();
            msv.MimSeqID = ms.MimSeqID;
            msv.Title = ms.Title;
            msv.IsLiked = (m_UOW.MimSeqLikeRepository.GetByID(ms.MimSeqID, User.Identity.GetUserName()) != null);
            msv.MimViews = ms.Mims
                .OrderBy(m => m.CreatedDate)
                .Select(m => new MimView() { MimID = m.MimID, Vote = this.GetVote(m), ViewMode = vm });
            msv.IsOwner = this.IsCurrentUserMimSeqOwner(ms);
            return msv;
        }

        private bool IsCurrentUserMimSeqOwner(MimSeq ms)
        {
            try
            {
                var mimseqOwner = ms.Mims.Single(m => m.PrevMimID == 0).GetCreatorName(userdb);
                return User.Identity.GetUserName().Equals(mimseqOwner, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private int GetVote(Mim m)
        {
            var mv = m_UOW.MimVoteRepository.GetByID(m.ID, User.Identity.GetUserName());
            return (mv == null) ? 0 : mv.Vote;
        }
    }
}
