using Mimry.Models;
using System;
using System.Web;

namespace Mimry.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Mim> MimRepository { get; }
        GenericRepository<MimSeq> MimSeqRepository { get; }
        GenericRepository<MimSeqLike> MimSeqLikeRepository { get; }
        GenericRepository<MimVote> MimVoteRepository { get; }
        GenericRepository<MimSeqComment> MimSeqCommentRepository { get; }
        GenericRepository<MimSeqCommentVote> MimSeqCommentVoteRepository { get; }
        void Save();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private const string c_UOWKey = "__MimryUnitOfWork__";
        private MimDBContext m_Context = new MimDBContext();
        private GenericRepository<Mim> m_MimRepository;
        private GenericRepository<MimSeq> m_MimSeqRepository;
        private GenericRepository<MimSeqLike> m_MimSeqLikeRepository;
        private GenericRepository<MimVote> m_MimVoteRepository;
        private GenericRepository<MimSeqComment> m_MimSeqCommentRepository;
        private GenericRepository<MimSeqCommentVote> m_MimSeqCommentVoteRepository;

        private bool m_Disposed = false;

        public static UnitOfWork Current
        {
            get 
            {
                UnitOfWork current = (UnitOfWork)HttpContext.Current.Items[c_UOWKey];

                if (current == null)
                {
                    HttpContext.Current.Items[c_UOWKey] = current = new UnitOfWork();
                }

                return current;
            }
        }

        public GenericRepository<Mim> MimRepository
        {
            get
            {

                if (this.m_MimRepository == null)
                {
                    this.m_MimRepository = new GenericRepository<Mim>(m_Context);
                }
                return m_MimRepository;
            }
        }

        public GenericRepository<MimSeq> MimSeqRepository
        {
            get
            {

                if (this.m_MimSeqRepository == null)
                {
                    this.m_MimSeqRepository = new GenericRepository<MimSeq>(m_Context);
                }
                return m_MimSeqRepository;
            }
        }

        public GenericRepository<MimSeqLike> MimSeqLikeRepository
        {
            get 
            {
                if (m_MimSeqLikeRepository == null)
                {
                    this.m_MimSeqLikeRepository = new GenericRepository<MimSeqLike>(m_Context);
                }
                return m_MimSeqLikeRepository;
            }
        }

        public GenericRepository<MimVote> MimVoteRepository
        {
            get
            {
                if (m_MimVoteRepository == null)
                {
                    this.m_MimVoteRepository = new GenericRepository<MimVote>(m_Context);
                }
                return m_MimVoteRepository;
            }
        }

        public GenericRepository<MimSeqComment> MimSeqCommentRepository
        {
            get 
            {
                if (m_MimSeqCommentRepository == null)
                {
                    this.m_MimSeqCommentRepository = new GenericRepository<MimSeqComment>(m_Context);
                }
                return m_MimSeqCommentRepository;
            }
        }

        public GenericRepository<MimSeqCommentVote> MimSeqCommentVoteRepository
        {
            get
            {
                if (m_MimSeqCommentVoteRepository == null)
                {
                    this.m_MimSeqCommentVoteRepository = new GenericRepository<MimSeqCommentVote>(m_Context);
                }
                return m_MimSeqCommentVoteRepository;
            }
        }

        public void Save()
        {
            m_Context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                if (disposing)
                {
                    m_Context.Dispose();
                }
            }
            this.m_Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}