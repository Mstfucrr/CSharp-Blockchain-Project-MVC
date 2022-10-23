using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using RecycleCoinMvc.Models;

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
            if (Request.HttpMethod == "POST")
            {
                try
                {
                    var fromAddress = Request.Form["fromAddress"];
                    var toAddress = Request.Form["toAddress"];
                    var amount = Request.Form["amount"];
                    var transactionContent = new JObject(new JProperty("fromAddress", fromAddress),
                        new JProperty("toAddress", toAddress),
                        new JProperty("amount", amount)).ToString();

                    var content = new StringContent(transactionContent, System.Text.Encoding.UTF8, "application/json");

                    var pushTransaction = httpClient.PostAsync("api/Transaction/AddTransaction", content);
                    pushTransaction.Result.EnsureSuccessStatusCode();

                    ViewBag.toast = new Toastr("İşlem", "İşleminiz başarıyla gerçekleştirildi", "success");
                }
                catch (Exception e)
                {
                    ViewBag.toast = new Toastr("İşlem", "İşleminiz Başarısız!", "danger");
                }

            }


            return View();
        }

        public ActionResult PendingTransaction()
        {
            var getBlockchain = httpClient.GetAsync("api/Transaction/getPendingTransactions");
            getBlockchain.Result.EnsureSuccessStatusCode();
            var res = getBlockchain.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            ViewBag.PendingTransactions = j_res;

            return View();
        }

    }
}