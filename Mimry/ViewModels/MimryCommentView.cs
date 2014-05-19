using System;
using System.ComponentModel.DataAnnotations;
using Mimry.Models;

namespace Mimry.ViewModels
{
    public class MimryCommentView
    {
        public int CommentID { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string User { get; set; }
        public string Value { get; set; }
        public int Vote { get; set; }
        public bool ShowEdit { get; set; }
    }
}