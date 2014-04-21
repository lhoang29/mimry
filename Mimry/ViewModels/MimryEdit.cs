using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryEdit
    {
        [Required]
        public Guid MimSeqID { get; set; }

        [Required]
        [Display(Name = "Mimry Title")]
        public string Title { get; set; }
    }
}