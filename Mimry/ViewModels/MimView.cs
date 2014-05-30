using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public enum MimViewMode 
    { 
        Thumbnail = 0,
        Medium,
        Full
    }
    public class MimView
    {
        public Guid MimSeqID { get; set; }
        public Guid MimID { get; set; }
        public int Vote { get; set; }
        public MimViewMode ViewMode { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static readonly int ContainerPadding = 20;
        public static readonly int AddViewSize = 250;

        private const int c_ThumbnailSize = 350;
        private const int c_MediumSize = 600;
        public static int GetMaxMimSize(MimViewMode mode)
        {
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
            return maxSize;
        }
    }
}