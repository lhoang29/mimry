using System;
using System.Net;
using System.Web.Mvc;

namespace Mimry.Helpers
{
    public static class ExtensionHelpers
    {
        public static void ValidateAddImage(this Controller c, Mimry.Models.Mim mim, string imageUrl)
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

            if (c.ModelState["Image"].Errors != null)
            {
                c.ModelState["Image"].Errors.Clear();
            }

            if (!String.IsNullOrEmpty(imageLoadError))
            {
                c.ModelState.AddModelError("Image", imageLoadError);
            }
        }
    }
}