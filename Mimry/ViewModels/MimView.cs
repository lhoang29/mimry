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

        private const int c_MinimumSize = 300;
        private const int c_ThumbnailSize = 350;
        private const int c_MediumSize = 600;

        /// <summary>
        /// The universal minimum size to display a meme regardless of its display mode.
        /// </summary>
        public static int MinimumSize
        {
            get { return c_MinimumSize; }
        } 

        /// <summary>
        /// Gets the maximum size of a meme for the specified view mode.
        /// </summary>
        /// <param name="mode">The meme view mode.</param>
        /// <returns>Maximum size (in pixels).</returns>
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

        /// <summary>
        /// Gets the meme container size for the specified view mode.
        /// </summary>
        /// <param name="mode">The meme view mode.</param>
        /// <returns>The size of the container (in pixels).</returns>
        public static int GetContainerSize(MimViewMode mode)
        {
            return MimView.GetMaxMimSize(mode) + MimView.ContainerPadding;
        }
    }
}