using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly BlockchainApi _blockchainApi;


        public HomeController()
        {
            _blockchainApi = new BlockchainApi();
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
    }
}