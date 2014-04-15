﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mimry.Models
{
    public class Mim
    {
        [Key]
        public int MimID { get; set; }
        [Display(Name="Mim Title")]
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
        
        [Required]
        [Display(Name = "Meme Image")]
        public byte[] Image { get; set; }

        [Display(Name = "Top Caption")]
        public string CaptionTop { get; set; }

        [Display(Name = "Bottom Caption")]
        public string CaptionBottom { get; set; }

        [Required]
        public int MimSeqID { get; set; }
        
        public virtual MimSeq MimSeq { get; set; }
    }
    public class MimSeq
    {
        [Key]
        public int MimSeqID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<Mim> Mims { get; set; }
    }
    public class MimDBContext : DbContext
    {
        public DbSet<Mim> Mims { get; set; }
        public DbSet<MimSeq> MimSeqs { get; set; }
    }
}