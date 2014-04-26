using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimDetails
    {
        [Required]
        public Guid MimID { get; set; }

        [Display(Name = "Mimry Title")]
        public string MimryTitle { get; set; }

        [Display(Name = "Mim Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Display(Name = "Last Modified")]
        public DateTime LastModifiedDate { get; set; }

        [Required]
        [Display(Name = "Created By")]
        public string Creator { get; set; }

        public bool IsOwner { get; set; }

        public bool IsEditable { get; set; }
    }
}