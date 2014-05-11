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
        public Guid MimID { get; set; }
        public int Vote { get; set; }
        public MimViewMode ViewMode { get; set; }
    }
}