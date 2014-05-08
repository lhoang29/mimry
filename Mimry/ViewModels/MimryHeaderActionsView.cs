using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public class MimryHeaderActionsView
    {
        public Guid MimSeqID { get; set; }
        public bool IsLiked { get; set; }
    }
}