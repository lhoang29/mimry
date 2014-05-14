using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimry.DAL;
using Mimry.Models;

namespace MimryUnitTests.MockDAL
{
    class MockUnitOfWork : IUnitOfWork
    {
        public IRepository<Mim> MimRepository
        {
            get { return new MockRepository<Mim>(); }
        }

        public IRepository<MimSeq> MimSeqRepository
        {
            get { return new MockRepository<MimSeq>(); }
        }

        public IRepository<MimSeqLike> MimSeqLikeRepository
        {
            get { return new MockRepository<MimSeqLike>(); }
        }

        public IRepository<MimVote> MimVoteRepository
        {
            get { return new MockRepository<MimVote>(); }
        }

        public IRepository<MimSeqComment> MimSeqCommentRepository
        {
            get { return new MockRepository<MimSeqComment>(); }
        }

        public IRepository<MimSeqCommentVote> MimSeqCommentVoteRepository
        {
            get { return new MockRepository<MimSeqCommentVote>(); }
        }

        public void Save() { }

        public void Dispose() { }
    }
}
