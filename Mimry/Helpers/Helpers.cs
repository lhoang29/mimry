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
                    mim.Image = imageData;
                }
            }
            catch (Exception ex)
            {
                imageLoadError = "Invalid URL, Mimry was unable to grab the image at the specified address, please double check";
            }

            if (c.ModelState[modelStateProperty].Errors != null)
            {
                c.ModelState[modelStateProperty].Errors.Clear();
            }

            if (!String.IsNullOrEmpty(imageLoadError))
            {
                c.ModelState.AddModelError(modelStateProperty, imageLoadError);
            }
        }

        public static void GenerateTestData(this Controller c, MimDBContext db)
        {
            int numMimries = 15;
            int numUsers = 8;
            int[] numMimRange = { 3, 20 };

            string userPrefix = "user";
            string userPassword = "112211";
            string mimryTitlePrefix = "Mimry Title ";
            string captionTopPrefix = "Top caption ";
            string captionBottomPrefix = "Bottom caption ";

            Random rand = new Random((int)DateTime.Now.Ticks);

            // Get list of mim images
            List<Tuple<string, byte[]>> imageData = new List<Tuple<string, byte[]>>();
            using (System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(@"C:\Users\lhoang\Documents\Git\Mimry\Mimry\TestData\mims.bin")))
            {
                try
                {
                    while (true)
                    {
                        string title = br.ReadString();
                        int bc = br.ReadInt32();
                        var imageBytes = br.ReadBytes(bc);
                        imageData.Add(new Tuple<string, byte[]>(title, imageBytes));
                    }
                }
                catch { }
            }

            // Create random users 
            List<ApplicationUser> userList = new List<ApplicationUser>();
            var userMananger = new UserManager<ApplicationUser>(new
                Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
            
            for (int i = 0; i < numUsers; i++)
            {
                var user = new ApplicationUser() { UserName = userPrefix + (i + 1) };
                var result = userMananger.CreateAsync(user, userPassword);
                userMananger.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                userList.Add(user);
            }

            // Create random mimries
            for (int i = 0; i < numMimries; i++)
            {
                MimSeq ms = new MimSeq();
                ms.Title = mimryTitlePrefix + (i + 1);
                db.MimSeqs.Add(ms);

                int numMims = rand.Next(numMimRange[0], numMimRange[1]);
                for (int j = 0; j < numMims; j++)
                {
                    Mim m = new Mim();
                    m.Creator = userList[rand.Next(numUsers)].Id;
                    m.CaptionTop = captionTopPrefix + (j + 1);
                    m.CaptionBottom = captionBottomPrefix + (j + 1);

                    int randImageIdx = rand.Next(imageData.Count);
                    m.Title = imageData[randImageIdx].Item1;
                    m.Image = imageData[randImageIdx].Item2;
                    m.MimSeq = ms;
                    db.Mims.Add(m);
                }
            }
            db.SaveChanges();
        }
    }
}