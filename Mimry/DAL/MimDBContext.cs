using Mimry.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace Mimry.DAL
{
    public class MimDBContext : DbContext
    {
        public DbSet<Mim> Mims { get; set; }
        public DbSet<MimSeq> MimSeqs { get; set; }
        public DbSet<MimSeqLike> MimSeqLikes { get; set; }
        public DbSet<MimVote> MimVotes { get; set; }
        public DbSet<MimSeqComment> MimSeqComments { get; set; }
        public DbSet<MimSeqCommentLike> MimSeqCommentLikes { get; set; }
        public DbSet<MimSeqCommentVote> MimSeqCommentVotes { get; set; }

        public override int SaveChanges()
        {
            foreach (var entity in ChangeTracker.Entries()
              .Where(p => p.State == EntityState.Added || p.State == EntityState.Modified))
            {
                if (entity.State == EntityState.Added)
                {
                    if (entity.Entity is IDateCreated)
                    {
                        ((IDateCreated)entity.Entity).CreatedDate = DateTime.Now;
                    }
                    if (entity.Entity is IDateModified)
                    {
                        ((IDateModified)entity.Entity).LastModifiedDate = DateTime.Now;
                    }
                }
                if (entity.State == EntityState.Modified && entity.Entity is IDateModified)
                {
                    ((IDateModified)entity.Entity).LastModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}