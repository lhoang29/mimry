using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;
using System.Collections.Generic;

namespace Mimry.ViewModels
{
    public class MimSeqPageView
    {
        public IEnumerable<MimSeqView> MimSeqViews { get; set; }
        public int PageIndex { get; set; }
    }
}