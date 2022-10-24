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

                    Session["toast"] = new Toastr("İşlem", "İşleminiz başarıyla gerçekleştirildi", "success");
                }
                catch (Exception e)
                {
                    Session["toast"] = new Toastr("İşlem", "İşleminiz Başarısız!", "danger");
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

        public ActionResult MinePendingTransaction(string minerRewardAddress)
        {
            try
            {
                var transactionContent = new JObject(new JProperty("minerRewardAddress", minerRewardAddress)).ToString();

                var content = new StringContent(transactionContent, System.Text.Encoding.UTF8, "application/json");

                var pushTransaction = httpClient.PostAsync("api/Transaction/minerPendingTransactions", content);
                pushTransaction.Result.EnsureSuccessStatusCode();

                Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarıyla gerçekleştirildi.", "success");

            }
            catch (Exception e)
            {

                Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarısız.", "danger");
                return RedirectToAction("PendingTransaction");

            }



            return RedirectToAction("Index", "Home");
        }

    }
}