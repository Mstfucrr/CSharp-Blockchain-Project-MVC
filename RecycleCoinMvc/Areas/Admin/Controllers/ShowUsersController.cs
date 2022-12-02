﻿using System.Linq;
using RecycleCoin.Business.Concrete;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoin.Entities.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    public class ShowUsersController : Controller
    {
        private readonly UserRecycleItemManager _userRecycleItemManager;
        private readonly UserManager<AppUser> _userManager;


        public ShowUsersController()
        {
            _userRecycleItemManager = new UserRecycleItemManager(new EfUserRecycleItemDal());
            var userStore = new UserStore<AppUser>(new RecycleCoinDbContext());

            _userManager = new UserManager<AppUser>(userStore);

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

        public ActionResult GetUserInfo(string userId)
        {
            var user = _userRecycleItemManager.GetList().First(u=>u.User.Id == userId).User;
            ViewBag.User = user;
            return PartialView("_userInfoPartial");
        }


        public ActionResult GetStatistic()
        {
            //istatisticler çekilecek

            return PartialView("_showStatisticsPartial");
        }
    }
}