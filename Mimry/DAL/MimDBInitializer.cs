using Microsoft.AspNet.Identity;
using Mimry.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;

namespace Mimry.DAL
{
    public class MimDBInitializer : CreateDatabaseIfNotExists<MimDBContext>
    {
        protected override void Seed(MimDBContext context)
        {
            MimDBInitializer.SeedData(context);
            base.Seed(context);
        }

        public static void SeedData(MimDBContext context)
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
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "mims.bin");
            using (var br = new BinaryReader(File.OpenRead(dataPath)))
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
                context.MimSeqs.Add(ms);

                int numMims = rand.Next(numMimRange[0], numMimRange[1]);
                Mim prevMim = null;
                for (int j = 0; j < numMims; j++)
                {
                    Mim m = new Mim();
                    m.Creator = userList[rand.Next(numUsers)].Id;
                    m.CaptionTop = captionTopPrefix + (j + 1);
                    m.CaptionBottom = captionBottomPrefix + (j + 1);

                    int randImageIdx = rand.Next(imageData.Count);
                    m.Title = imageData[randImageIdx].Item1;
                    m.Image = Convert.ToBase64String(imageData[randImageIdx].Item2);
                    using (MemoryStream memStream = new MemoryStream(imageData[randImageIdx].Item2))
                    {
                        using (Bitmap bm = new Bitmap(memStream))
                        {
                            m.Width = bm.Width;
                            m.Height = bm.Height;
                        }
                    }
                    m.MimSeq = ms;
                    context.Mims.Add(m);
                    context.SaveChanges();
                    if (prevMim != null)
                    {
                        m.PrevMimID = prevMim.ID;
                        prevMim.NextMimID = m.ID;
                    }
                    prevMim = m;
                }
            }
            context.SaveChanges();
        }
    }
}