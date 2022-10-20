using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace RecycleCoinMvc.Controllers
{
    public class TransactionController : Controller
    {
        private readonly HttpClient httpClient;


        public TransactionController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
        }
        
        // GET: Transaction
        public ActionResult CreateTransaction()
        {
            return View();
        }

        public ActionResult PendingTransaction()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetTransactionByBlock(string hash)
        {
            Session.Clear();
            var getBlockchain = httpClient.GetAsync("api/getBlockByHash/"+hash);
            getBlockchain.Result.EnsureSuccessStatusCode();
            var res = getBlockchain.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            Session["blockByHash"] = j_res;
            return RedirectToAction("Index", "Home");

        }

    }
}