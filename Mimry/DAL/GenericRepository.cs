using Mimry.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mimry.DAL
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetByID(params object[] id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
    }
    public class GenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        internal MimDBContext m_Context;
        internal DbSet<TEntity> m_DbSet;

        public GenericRepository(MimDBContext context)
        {
            this.m_Context = context;
            this.m_DbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = m_DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public TEntity GetByID(params object[] id)
        {
            return m_DbSet.Find(id);
        }

        public void Insert(TEntity entity)
        {
            m_DbSet.Add(entity);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = m_DbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (m_Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                m_DbSet.Attach(entityToDelete);
            }
            m_DbSet.Remove(entityToDelete);
        }

        public void Update(TEntity entityToUpdate)
        {
            m_DbSet.Attach(entityToUpdate);
            m_Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}