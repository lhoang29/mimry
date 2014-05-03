using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryHeader
    {
        [Required]
        public Guid MimSeqID { get; set; }

        [Required]
        public string Title { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsLiked { get; set; }
        public bool IsOwner { get; set; }
    }
}