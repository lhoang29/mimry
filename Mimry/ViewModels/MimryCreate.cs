using System.ComponentModel.DataAnnotations;

namespace Mimry.ViewModels
{
    public class MimryCreate
    {
        [Required(ErrorMessage = "Please enter a title.")]
        public string MimryTitle { get; set; }

        public string MimTitle { get; set; }

        [Required(ErrorMessage = "Please enter a URL.")]
        public string ImageUrl { get; set; }

        public string CaptionTop { get; set; }
        public string CaptionBottom { get; set; }

        public string ReturnUrl { get; set; }
    }
}