using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mimry.Models
{
    public class MimSeqComment : IUserAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }
        [Required]
        public virtual MimSeq MimSeq { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string User { get; set; }
        public virtual MimSeqComment Parent { get; set; }
        [Required]
        public string Value { get; set; }
    }

    public class MimSeqCommentLike : IUserAction
    {
        [Key, Column(Order = 1)]
        public int MimSeqCommentID { get; set; }
        [Key, Column(Order = 2)]
        [Required(AllowEmptyStrings = false)]
        public string User { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

    public class MimSeqCommentVote : IUserAction
    {
        [Key, Column(Order = 1)]
        public int MimSeqCommentID { get; set; }
        [Key, Column(Order = 2)]
        [Required(AllowEmptyStrings = false)]
        public string User { get; set; }
        public int Vote { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}