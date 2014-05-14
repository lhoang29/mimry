using Microsoft.AspNet.Identity;
using Mimry.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace Mimry.Helpers
{
    public static class ExtensionHelpers
    {
        public static void ValidateAddImage(this Controller c, Mimry.Models.Mim mim, string imageUrl, string modelStateProperty)
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
                    imageLoadError = "Invalid URL, Mimry was unable to grab the image at the specified address, please double check";
                }
                else
                {
                    mim.Image = Convert.ToBase64String(imageData);
                }
            }
            catch (Exception ex)
            {
                imageLoadError = "Invalid URL, Mimry was unable to grab the image at the specified address, please double check";
            }

            if (c.ModelState[modelStateProperty] != null && 
                c.ModelState[modelStateProperty].Errors != null)
            {
                c.ModelState[modelStateProperty].Errors.Clear();
            }

            if (!String.IsNullOrEmpty(imageLoadError))
            {
                c.ModelState.AddModelError(modelStateProperty, imageLoadError);
            }
        }
    }
}