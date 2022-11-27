using RecycleCoin.Entities.Concrete;
using System.Collections.Generic;

namespace RecycleCoin.DataAccess.Abstract
{
    public interface IUserRecycleItemDal: IEntityRepositoryBase<UserRecycleItem>
    {
        List<UserRecycleItem> GetListByUser(string userId);
    }
}
