using Mimry.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Mimry.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal MimDBContext m_Context;
        internal DbSet<TEntity> m_DbSet;

        public GenericRepository(MimDBContext context)
        {
            this.m_Context = context;
            this.m_DbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
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

        public virtual TEntity GetByID(params object[] id)
        {
            return m_DbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            m_DbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = m_DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (m_Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                m_DbSet.Attach(entityToDelete);
            }
            m_DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            m_DbSet.Attach(entityToUpdate);
            m_Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}