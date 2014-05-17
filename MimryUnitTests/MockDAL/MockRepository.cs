using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimry.DAL;
using System.Linq.Expressions;

namespace MimryUnitTests.MockDAL
{
    public class MockRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public IQueryable<TEntity> GetQuery()
        {
            return null;
        }
        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            return null;
        }
        public TEntity GetByID(params object[] id)
        {
            return null;
        }
        public void Insert(TEntity entity) { }
        public void Delete(object id) { }
        public void Delete(TEntity entityToDelete) { }

        public void Update(TEntity entityToUpdate) { }
    }
}
