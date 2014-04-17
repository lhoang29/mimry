using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryCreate
    {
        [Display(Name = "Mimry Title")]
        public string MimryTitle { get; set; }

        [Display(Name = "Mim Title")]
        public string MimTitle { get; set; }

        [Required]
        [Display(Name = "Meme Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Top Caption")]
        public string CaptionTop { get; set; }

        [Display(Name = "Bottom Caption")]
        public string CaptionBottom { get; set; }
    }
}