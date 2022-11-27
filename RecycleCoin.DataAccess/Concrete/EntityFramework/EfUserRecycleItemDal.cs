using System.Collections.Generic;
using RecycleCoin.DataAccess.Abstract;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Repositories.Concrete;
using RecycleCoin.Entities.Concrete;

namespace RecycleCoin.DataAccess.Concrete.EntityFramework
{
    public class EfUserRecycleItemDal:EfEntityRepositoryBase<UserRecycleItem>, IUserRecycleItemDal
    {
        private RecycleCoinDbContext _recycleCoinDbContext;
        public List<UserRecycleItem> GetListByUser(string userId)
        {
            _recycleCoinDbContext = new RecycleCoinDbContext();
           var userRecycleItems = new List<UserRecycleItem>(_recycleCoinDbContext.UserRecycleItems.SqlQuery($@"call recyclecoindb.sp_getUserRecycleItemsByUserId('{userId}');"));
           return userRecycleItems ?? null;
        }
    }
}
