using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryContinue
    {
        [Required]
        public Guid MimSeqID { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a URL.")]
        public string ImageUrl { get; set; }

        public string ReturnUrl { get; set; }
    }
}