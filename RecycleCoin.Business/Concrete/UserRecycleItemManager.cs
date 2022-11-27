﻿using System.Collections.Generic;
using RecycleCoin.Business.Abstract;
using RecycleCoin.DataAccess.Abstract;
using RecycleCoin.Entities.Concrete;

namespace RecycleCoin.Business.Concrete
{
    public class UserRecycleItemManager : IUserRecycleItemService
    {
        private readonly IUserRecycleItemDal _userRecycleItemDal;

        public UserRecycleItemManager(IUserRecycleItemDal _userRecycleItemDal)
        {
            this._userRecycleItemDal = _userRecycleItemDal;
        }

        public List<UserRecycleItem> GetList()
        {
            return _userRecycleItemDal.GetList();
        }

        public List<UserRecycleItem> GetListByUserId(string userId)
        {
            return _userRecycleItemDal.GetListByUser(userId);
        }

        public void Add(UserRecycleItem userRecycleItem)
        {
            _userRecycleItemDal.Add(userRecycleItem);
        }
    }
}