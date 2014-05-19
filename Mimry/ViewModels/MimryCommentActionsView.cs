using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public class MimryCommentActionsView
    {
        public int CommentID { get; set; }
        public int Vote { get; set; }
        public bool ShowEdit { get; set; }
    }
}