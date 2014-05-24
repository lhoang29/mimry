using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryEdit
    {
        [Required]
        public Guid MimSeqID { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; }

        public string ReturnUrl { get; set; }
    }
}