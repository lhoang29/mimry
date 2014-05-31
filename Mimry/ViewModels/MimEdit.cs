using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimEdit
    {
        [Required]
        public Guid MimID { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a URL.")]
        public string ImageUrl { get; set; }
    }
}