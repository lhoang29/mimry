using Mimry.Models;
using System;
using System.Web;

namespace Mimry.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Mim> MimRepository { get; }
        IRepository<MimSeq> MimSeqRepository { get; }
        IRepository<MimSeqLike> MimSeqLikeRepository { get; }
        IRepository<MimVote> MimVoteRepository { get; }
        IRepository<MimSeqComment> MimSeqCommentRepository { get; }
        IRepository<MimSeqCommentVote> MimSeqCommentVoteRepository { get; }
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

        public IRepository<Mim> MimRepository
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

        public IRepository<MimSeq> MimSeqRepository
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

        public IRepository<MimSeqLike> MimSeqLikeRepository
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

        public IRepository<MimVote> MimVoteRepository
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

        public IRepository<MimSeqComment> MimSeqCommentRepository
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

        public IRepository<MimSeqCommentVote> MimSeqCommentVoteRepository
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