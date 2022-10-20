using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RecycleCoinMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;


        public HomeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
        }


        public ActionResult Index()
        {
            var getBlockchain = httpClient.GetAsync("api/getBlockchain");
            getBlockchain.Result.EnsureSuccessStatusCode();
            var res = getBlockchain.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            ViewBag.blockchain = j_res;
            if (Session["blockByHash"] != null)
            {
                ViewBag.blockByHash = Session["blockByHash"];
            }
            return View();
        }

        public ActionResult About()
        {
            var deneme = httpClient.GetAsync("api/getBlockchain");
            deneme.Result.EnsureSuccessStatusCode();
            var res = deneme.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            ViewBag.message = j_res;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}