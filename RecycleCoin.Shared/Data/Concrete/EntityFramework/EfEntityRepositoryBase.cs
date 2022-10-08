using RecycleCoin.Entities.Abstract;
using RecycleCoin.Shared.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecycleCoin.Shared.Data.Concrete.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        public EfEntityRepositoryBase()
        {
                
        }
        public void Delete(TEntity t)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetByFilter(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<TEntity> GetList()
        {
            throw new NotImplementedException();
        }

        public void Insert(TEntity t)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity t)
        {
            throw new NotImplementedException();
        }
    }
}
