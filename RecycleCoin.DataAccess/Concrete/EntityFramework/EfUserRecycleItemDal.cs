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

        public EfUserRecycleItemDal()
        {
            _recycleCoinDbContext = new RecycleCoinDbContext();

        }
        public List<UserRecycleItem> GetListByUser(string userId)
        {
           var userRecycleItems = new List<UserRecycleItem>(_recycleCoinDbContext.UserRecycleItems.SqlQuery($@"call recyclecoindb.sp_getUserRecycleItemsByUserId('{userId}');"));
           return userRecycleItems ?? null;
        }

        public List<UserRecycleItem> GetListOrderByDate()
        {
            var userRecycleItems = new List<UserRecycleItem>(_recycleCoinDbContext.UserRecycleItems.SqlQuery($@"Select * from recyclecoindb.userrecycleitems ur order by ur.RecycleDate;"));
            return userRecycleItems ?? null;
        }

        public List<UserRecycleItem> GetListOrderByDateDesc()
        {
            var userRecycleItems = new List<UserRecycleItem>(_recycleCoinDbContext.UserRecycleItems.SqlQuery($@"Select * from recyclecoindb.userrecycleitems ur order by ur.RecycleDate desc;"));
            return userRecycleItems ?? null;
        }


    }
}
