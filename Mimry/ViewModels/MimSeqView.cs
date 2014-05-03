using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;
using System.Collections.Generic;

namespace Mimry.ViewModels
{
    public class MimSeqView
    {
        public Guid MimSeqID { get; set; }
        public string Title { get; set; }
        public bool IsLiked { get; set; }
        public bool IsOwner { get; set; }
        public IEnumerable<MimView> MimViews { get; set; }
    }
}