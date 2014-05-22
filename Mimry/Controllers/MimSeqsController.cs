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
using Mimry.Attributes;

namespace Mimry.Controllers
{
    [AjaxAuthorize("/Account/Login/")]
    public class MimSeqsController : Controller
    {
        private IUnitOfWork m_UOW;
        private const int c_PageSize = 3; // # of mimries to load

        public MimSeqsController() : this(UnitOfWork.Current) { }

        public MimSeqsController(IUnitOfWork uow)
        {
            m_UOW = uow;
        }

        // GET: /MimSeqs/
        [AllowAnonymous]
        public ActionResult Index(int page = 0)
        {
            MimSeqPageView msPageData = null;
            try
            {
                string currentUserName = User.Identity.GetUserName();
                var data = m_UOW.MimSeqRepository.GetQuery()
                    .OrderBy(ms => ms.CreatedDate)
                    .Skip(page * c_PageSize).Take(c_PageSize)
                    .Select(ms => new
                {
                    ID = ms.MimSeqID,
                    Title = ms.Title,
                    LikeCount = ms.Likes.Count,
                    CommentCount = ms.Comments.Count,
                    Like = Request.IsAuthenticated
                        ? ms.Likes.FirstOrDefault(l => l.User == currentUserName)
                        : null,
                    Owner = Request.IsAuthenticated
                        ? ms.Mims.FirstOrDefault(m => m.PrevMimID == 0).Creator
                        : null,
                    Mims = ms.Mims.OrderBy(m => m.CreatedDate).Select(m => new
                    {
                        MimID = m.MimID,
                        MimVote = Request.IsAuthenticated
                            ? m.Votes.FirstOrDefault(v => v.User == currentUserName)
                            : null
                    })
                }).ToList();

                var mimSeqViews = new List<MimSeqView>();
                foreach (var item in data)
                {
                    MimSeqView msv = new MimSeqView();
                    msv.MimSeqID = item.ID;
                    msv.Title = item.Title;
                    msv.LikeCount = item.LikeCount;
                    msv.CommentCount = item.CommentCount;
                    msv.IsOwner = item.Owner != null && item.Owner.Equals(currentUserName, StringComparison.OrdinalIgnoreCase);
                    msv.IsLiked = (item.Like != null);
                    var mimViews = new List<MimView>();
                    foreach (var mv in item.Mims)
                    {
                        MimView mimView = new MimView();
                        mimView.MimID = mv.MimID;
                        mimView.ViewMode = MimViewMode.Thumbnail;
                        mimView.Vote = (mv.MimVote != null) ? mv.MimVote.Vote : 0;
                        mimViews.Add(mimView);
                    }
                    msv.MimViews = mimViews;
                    mimSeqViews.Add(msv);
                }
                if (mimSeqViews.Count > 0)
                {
                    msPageData = new MimSeqPageView()
                    {
                        MimSeqViews = mimSeqViews,
                        PageIndex = page + 1
                    };
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("MimryIndexContent", msPageData);
            }
            else
            {
                return View(msPageData);
            }
        }

        // GET: /MimSeqs/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MimSeqView msv = null;
            try
            {
                string currentUserName = User.Identity.GetUserName();
                var data = m_UOW.MimSeqRepository.GetQuery()
                .Where(ms => ms.MimSeqID == id)
                .Select(ms => new
                {
                    ID = ms.MimSeqID,
                    Title = ms.Title,
                    LikeCount = ms.Likes.Count,
                    Like = Request.IsAuthenticated
                        ? ms.Likes.FirstOrDefault(l => l.User == currentUserName)
                        : null,
                    Owner = Request.IsAuthenticated
                        ? ms.Mims.FirstOrDefault(m => m.PrevMimID == 0).Creator
                        : null,
                    Mims = ms.Mims.OrderBy(m => m.CreatedDate).Select(m => new
                    {
                        MimID = m.MimID,
                        MimVote = Request.IsAuthenticated
                            ? m.Votes.FirstOrDefault(v => v.User == currentUserName)
                            : null
                    }),
                    Comments = ms.Comments.Select(c => new
                    {
                        CommentID = c.CommentID,
                        LastModifiedDate = c.LastModifiedDate,
                        User = c.User,
                        Value = c.Value,
                        Vote = Request.IsAuthenticated ? c.Votes.FirstOrDefault(v => v.User == currentUserName) : null
                    })
                }).ToList();

                var msvData = data[0];

                msv = new MimSeqView();
                msv.MimSeqID = msvData.ID;
                msv.Title = msvData.Title;
                msv.LikeCount = msvData.LikeCount;
                msv.IsOwner = msvData.Owner != null && msvData.Owner.Equals(currentUserName, StringComparison.OrdinalIgnoreCase);
                msv.IsLiked = (msvData.Like != null);
                var mimViews = new List<MimView>();
                foreach (var mv in msvData.Mims)
                {
                    MimView mimView = new MimView();
                    mimView.MimID = mv.MimID;
                    mimView.ViewMode = MimViewMode.Medium;
                    mimView.Vote = (mv.MimVote != null) ? mv.MimVote.Vote : 0;
                    mimViews.Add(mimView);
                }
                msv.MimViews = mimViews;

                var commentViews = new List<MimryCommentView>();
                foreach (var cv in msvData.Comments)
                {
                    var mcv = new MimryCommentView();
                    mcv.CommentID = cv.CommentID;
                    mcv.LastModifiedDate = cv.LastModifiedDate;
                    mcv.ShowEdit = cv.User.Equals(currentUserName, StringComparison.OrdinalIgnoreCase);
                    mcv.User = cv.User;
                    mcv.Value = cv.Value;
                    mcv.Vote = (cv.Vote != null) ? cv.Vote.Vote : 0;
                    commentViews.Add(mcv);
                }
                msv.CommentViews = commentViews;
                msv.CommentCount = commentViews.Count;
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return View(msv);
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

                mim.Creator = User.Identity.GetUserName();
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
        [ValidateAntiForgeryToken]
        public ActionResult Like(Guid id)
        {
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }

            string userName = User.Identity.GetUserName();
            if (String.IsNullOrWhiteSpace(userName))
            {
                // If code can get in here with an invalid user name then there's an internal server error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            MimSeqLike msl = m_UOW.MimSeqLikeRepository.GetByID(id, userName);
            bool isLiked = (msl == null);
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

            int likeCount = m_UOW.MimSeqLikeRepository.Get(ml => ml.MimSeqID == id).Count();

            return PartialView(
                "MimryHeaderActions", 
                new MimryHeaderActionsView() 
                { 
                    MimSeqID = mimseq.MimSeqID,
                    IsLiked = isLiked,
                    LikeCount = likeCount
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(Guid id, string txtComment)
        {
            MimSeq mimseq = m_UOW.MimSeqRepository.GetByID(id);
            if (mimseq == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrWhiteSpace(txtComment))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string userName = User.Identity.GetUserName();
            if (String.IsNullOrWhiteSpace(userName))
            {
                // If code can get in here with an invalid user name then there's an internal server error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            MimSeqComment msc = new MimSeqComment();
            msc.MimSeq = mimseq;
            msc.User = userName;
            msc.Value = txtComment;
            m_UOW.MimSeqCommentRepository.Insert(msc);
            m_UOW.Save();

            MimryCommentView mcv = new MimryCommentView()
            {
                Value = txtComment,
                User = userName,
                CommentID = msc.CommentID,
                LastModifiedDate = msc.LastModifiedDate,
                Vote = 0,
                ShowEdit = true
            };
            return PartialView("MimryComment", mcv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(int id, string txtComment)
        {
            MimSeqComment msc = m_UOW.MimSeqCommentRepository.GetByID(id);
            if (msc == null || msc.MimSeq == null)
            {
                return HttpNotFound();
            }

            if (String.IsNullOrWhiteSpace(txtComment))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            msc.Value = txtComment;
            m_UOW.MimSeqCommentRepository.Update(msc);
            m_UOW.Save();

            return Content(txtComment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoteComment(int id, string vote)
        {
            var mscs = m_UOW.MimSeqCommentRepository.Get(msc => msc.CommentID == id);
            if (mscs == null)
	        {
                return HttpNotFound();
	        }
            MimSeqComment mc = null;
            try
            {
                mc = mscs.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            string userName = User.Identity.GetUserName();
            if (String.IsNullOrWhiteSpace(userName))
            {
                // If code can get in here with an invalid user name then there's an internal server error
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            MimSeqCommentVote mscv = m_UOW.MimSeqCommentVoteRepository.GetByID(mc.CommentID, userName);
            if (mscv == null)
            {
                mscv = new MimSeqCommentVote() {
                    MimSeqCommentID = id,
                    Vote = Convert.ToInt32(vote),
                    User = userName
                };
                m_UOW.MimSeqCommentVoteRepository.Insert(mscv);
            }
            else
            {
                mscv.Vote = Convert.ToInt32(vote);
                m_UOW.MimSeqCommentVoteRepository.Update(mscv);
            }
            m_UOW.Save();

            bool isCurrentUserCommentOwner = mc.User.Equals(userName, StringComparison.OrdinalIgnoreCase);
            return PartialView("MimryCommentActions", new MimryCommentActionsView() { 
                CommentID = id,
                Vote = mscv.Vote,
                ShowEdit = isCurrentUserCommentOwner
            });
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
                var firstMim = m_UOW.MimRepository.Get(m => m.MimSeqID == mc.MimSeqID && m.NextMimID == 0);
                // A mimry should always have at least 1 Mim associated with it
                if (firstMim == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                mim.Creator = User.Identity.GetUserName();
                mim.MimSeqID = mc.MimSeqID;
                mim.Title = mc.Title;
                mim.CaptionTop = mc.CaptionTop;
                mim.CaptionBottom = mc.CaptionBottom;

                Mim prevMim = firstMim.Single();
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

        private bool IsCurrentUserMimSeqOwner(MimSeq ms)
        {
            try
            {
                var mimseqOwner = ms.Mims.Single(m => m.PrevMimID == 0).Creator;
                return User.Identity.GetUserName().Equals(mimseqOwner, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
