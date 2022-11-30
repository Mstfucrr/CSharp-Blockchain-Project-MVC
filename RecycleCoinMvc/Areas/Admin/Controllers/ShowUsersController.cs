﻿using System.Collections.Generic;
using System.Linq;
using RecycleCoin.Business.Concrete;
using System.Web.Mvc;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoin.Entities.Concrete;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    public class ShowUsersController : Controller
    {
        private readonly UserRecycleItemManager _userRecycleItemManager;

        public ShowUsersController()
        {
            _userRecycleItemManager = new UserRecycleItemManager(new EfUserRecycleItemDal());
        }
        // GET: Admin/ShowUsers
        public ActionResult Index()
        {
            var userRecycleItemInfos = _userRecycleItemManager.GetList();
            
            ViewBag.UserRecycleItemInfos = userRecycleItemInfos;
            return View();
        }

        public ActionResult OrderBy(string filter)
        {
            var userRecycleItemInfos = _userRecycleItemManager.GetListOrderBy(filter);
            ViewBag.UserRecycleItemInfos = userRecycleItemInfos;

            return PartialView("_userRecycleItemsPartial");
        }

        public ActionResult OrderByDesc(string filter)
        {
            var userRecycleItemInfos = _userRecycleItemManager.GetListOrderByDesc(filter);
            ViewBag.UserRecycleItemInfos = userRecycleItemInfos;
            return PartialView("_userRecycleItemsPartial");
        }
    }
}