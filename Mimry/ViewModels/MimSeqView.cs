using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public class MimSeqView
    {
        public MimSeq MimSeq { get; set; }
        public bool IsLiked { get; set; }
    }
}