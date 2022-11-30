﻿using RecycleCoin.Entities.Concrete;
using System.Collections.Generic;

namespace RecycleCoin.Business.Abstract
{
    public interface IUserRecycleItemService
    {
        List<UserRecycleItem> GetList();
        List<UserRecycleItem> GetListByUserId(string userId);
        List<UserRecycleItem> GetListOrderByDate();
        List<UserRecycleItem> GetListOrderByDateDesc();
        void Add(UserRecycleItem userRecycleItem);
    }
}
