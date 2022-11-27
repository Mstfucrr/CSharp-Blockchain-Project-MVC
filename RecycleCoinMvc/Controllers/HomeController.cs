using System;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using RecycleCoin.Business.Concrete;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly BlockchainApi _blockchainApi;
        private readonly CategoryManager _categoryManager;


        public HomeController()
        {
            _blockchainApi = new BlockchainApi();
            _categoryManager = new CategoryManager(new EfCategoryDal());
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var j_res = _blockchainApi.Get("api/getBlockchain");
            ViewBag.message = j_res;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Listener()
        {
            try
            {
                var j_res = _blockchainApi.Get("api/Blockchain/getBlockchain");
                ViewBag.blockchain = j_res;
                if (Session["blockByHash"] != null)
                {
                    ViewBag.blockByHash = Session["blockByHash"];
                }
            }
            catch (Exception)
            {
                ViewBag.blockchain = null;

            }

            return PartialView("_blcoksListenerPartial");
        }
        
        public ActionResult CategoryListener()
        {
            //var categories = _categoryManager.GetProductListByCarbonDesc();
            //ViewBag.categories = categories;
            return PartialView("_categoryListenerPartial");
        }

    }
}