using System;
using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryContinue
    {
        [Required]
        public Guid MimSeqID { get; set; }

        [Display(Name = "Mim Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Meme Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Top Caption")]
        public string CaptionTop { get; set; }

        [Display(Name = "Bottom Caption")]
        public string CaptionBottom { get; set; }
    }
}