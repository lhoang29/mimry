using System;
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
using ImageMagick;
using System.Globalization;

namespace Mimry.Controllers
{
    [AjaxAuthorize("/Account/Login/")]
    public class MimsController : Controller
    {
        private IUnitOfWork m_UOW;
        private const int c_ThumbnailSize = 350;
        private const int c_MediumSize = 600;

        public MimsController() : this(UnitOfWork.Current) { }

        public MimsController(IUnitOfWork uow)
        {
            m_UOW = uow;
        }

        // GET: /Mims/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mims = m_UOW.MimRepository.Get(m => m.MimID == id);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                mim = mims.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            MimDetails md = new MimDetails();
            md.MimID = mim.MimID;
            md.MimryTitle = mim.MimSeq.Title;
            md.Title = mim.Title;
            md.CreatedDate = mim.CreatedDate;
            md.LastModifiedDate = mim.LastModifiedDate;
            md.Creator = mim.Creator;
            md.IsOwner = String.Compare(User.Identity.GetUserName(), mim.Creator, StringComparison.OrdinalIgnoreCase) == 0;
            md.IsEditable = (mim.NextMimID == 0);

            return View(md);
        }

        [AllowAnonymous]
        public ActionResult Mimage(Guid? id, bool caption = false, MimViewMode mode = MimViewMode.Thumbnail)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mims = m_UOW.MimRepository.GetQuery()
                .Where(m => m.MimID == id)
                .Select(m => new { 
                    Image = m.Image, 
                    LastModifiedDate = m.LastModifiedDate 
                });
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                var aMim = mims.Single();
                mim = new Mim() { 
                    Image = aMim.Image,
                    LastModifiedDate = aMim.LastModifiedDate
                };
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // Enable caching if image hasn't been modified since last time requested
            if (!String.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var lastModifiedDate = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "r", provider).ToLocalTime();
                if (lastModifiedDate >= mim.LastModifiedDate.AddMilliseconds(-mim.LastModifiedDate.Millisecond))
                {
                    Response.StatusCode = 304;
                    Response.StatusDescription = "Not Modified";
                    return Content(String.Empty);
                }
            }

            byte[] imageData = Convert.FromBase64String(mim.Image);

            int maxSize = 0;
            switch (mode)
            {
                case MimViewMode.Thumbnail:
                    maxSize = c_ThumbnailSize;
                    break;
                case MimViewMode.Medium:
                    maxSize = c_MediumSize;
                    break;
            }

            string jpegType = "image/jpeg";
            string webpType = "image/webp";
            string contentType = String.Empty;
            using (MagickImage mi = new MagickImage(imageData))
            {
                if (maxSize > 0)
                {
                    double percentageResize = (double)maxSize / Math.Max(mi.Width, mi.Height);
                    if (percentageResize < 1)
                    {
                        mi.Sample(new Percentage(percentageResize * 100));
                    }
                    mi.Quality = 75;
                }
                else
                {
                    mi.Quality = 85;
                }
                if (Request.AcceptTypes.Contains(webpType))
                {
                    mi.Format = MagickFormat.WebP; // WebP format
                    contentType = webpType;
                }
                else
                {
                    mi.Format = MagickFormat.Pjpeg; // Progressive format
                    contentType = jpegType;
                }
                using (MemoryStream msb = new MemoryStream())
                {
                    mi.Write(msb);
                    imageData = msb.ToArray();
                }
            }

            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetLastModified(mim.LastModifiedDate);
            Response.Cache.SetMaxAge(TimeSpan.FromDays(60));

            return base.File(imageData, contentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(Guid id, string vote)
        {
            var mims = m_UOW.MimRepository.Get(m => m.MimID == id);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                mim = mims.Single();
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

        // GET: /Mims/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mims = m_UOW.MimRepository.Get(m => m.MimID == id);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                mim = mims.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            if (String.Compare(User.Identity.GetUserName(), mim.Creator, StringComparison.OrdinalIgnoreCase) != 0)
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
            var mims = m_UOW.MimRepository.Get(m => m.MimID == mim.MimID);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim currentMim = null;
            try
            {
                currentMim = mims.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mims = m_UOW.MimRepository.Get(m => m.MimID == id);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                mim = mims.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return View(mim);
        }

        // POST: /Mims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mims = m_UOW.MimRepository.Get(m => m.MimID == id);
            if (mims == null)
            {
                return HttpNotFound();
            }
            Mim mim = null;
            try
            {
                mim = mims.Single();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            int prevMimID = mim.PrevMimID;
            MimSeq mimSeq = mim.MimSeq;
            // Entity Framework sets up cascade delete by default so simply deleting the parent object will delete all 1-many foreign key related objects.
            m_UOW.MimRepository.Delete(mim);
            if (prevMimID > 0)
            {
                Mim prevMim = m_UOW.MimRepository.GetByID(prevMimID);
                if (prevMim != null)
                {
                    prevMim.NextMimID = 0;
                    m_UOW.MimRepository.Update(prevMim);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                // If this is the only Mim left, delete the Mimry altogether.
                m_UOW.MimSeqRepository.Delete(mimSeq);
            }

            m_UOW.Save();
            return RedirectToAction("Index", "MimSeqs");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_UOW.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Resize an image so that its width or height is no larger than 
        /// the specified value while keeping aspect ratio.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="maxScaleSize">The maximum size in either width or height.</param>
        /// <returns>New resized image.</returns>
        /// <remarks>
        /// If the specified image is already smaller than the specified max size
        /// then nothing is done.
        /// </remarks>
        public static Bitmap Resize(Bitmap image, int maxScaleSize)
        {
            int maxSize = Math.Max(image.Width, image.Height);
            if (maxSize > maxScaleSize)
            {
                float scale = (float)maxScaleSize / maxSize;
                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);
                Bitmap result = new Bitmap(scaleWidth, scaleHeight);
                using (var graph = Graphics.FromImage(result))
                {
                    graph.DrawImage(image, 0, 0, scaleWidth, scaleHeight);
                }
                return result;
            }
            return image;
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
