using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Mimry.Models
{
    public class Mim : IDateCreated, IDateModified
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MimID { get; set; }

        [Display(Name="Mim Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Display(Name = "Last Modified")]
        public DateTime LastModifiedDate { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Created By")]
        public string Creator { get; set; }
        
        [Required]
        [Display(Name = "Meme Image")]
        public string Image { get; set; }

        [Display(Name = "Top Caption")]
        public string CaptionTop { get; set; }

        [Display(Name = "Bottom Caption")]
        public string CaptionBottom { get; set; }

        public int NextMimID { get; set; }

        public int PrevMimID { get; set; }

        [Required]
        public Guid MimSeqID { get; set; }
        
        public virtual MimSeq MimSeq { get; set; }

        public string GetCreatorName(ApplicationDbContext db)
        { 
            ApplicationUser user = db.Users.Find(this.Creator);
            if (user != null)
            {
                return user.UserName;
            }
            else
            {
                return this.Creator;
            }
        }
    }
    public class MimSeq : IDateCreated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MimSeqID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<Mim> Mims { get; set; }
        public virtual ICollection<MimSeqComment> Comments { get; set; }
    }
    public class MimSeqLike : IUserAction
    {
        [Key, Column(Order = 1)]
        public Guid MimSeqID { get; set; }
        [Key, Column(Order = 2)]
        [Required(AllowEmptyStrings = false)]
        public string User { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

    public class MimVote : IUserAction
    {
        [Key, Column(Order = 1)]
        public int MimID { get; set; }
        [Key, Column(Order = 2)]
        [Required(AllowEmptyStrings = false)]
        public string User { get; set; }
        public int Vote { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}