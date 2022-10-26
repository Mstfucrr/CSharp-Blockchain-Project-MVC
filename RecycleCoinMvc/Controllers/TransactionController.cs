using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
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


                    var contentCheck = new StringContent(new JObject(new JProperty("Address", toAddress)).ToString(), System.Text.Encoding.UTF8, "application/json");
                    // Bakiye kısmı
                    var getWallet = httpClient.PostAsync("api/User/getBalanceOfAddress", contentCheck);
                    getWallet.Result.EnsureSuccessStatusCode();
                    var res_balance = getWallet.Result.Content.ReadAsStringAsync().Result;
                    var balance_of_toAddress = Convert.ToInt32(JsonConvert.DeserializeObject(res_balance));
                    var amount = Request.Form["amount"];

                    if ((balance_of_toAddress + Convert.ToInt32(amount)) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                    {
                        Session["toast"] = new Toastr("İşlem", "İşleminiz başarısız RC sınırı aşıldı!", "danger");
                        return View();
                    }

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

                var contentCheck = new StringContent(new JObject(new JProperty("Address", minerRewardAddress)).ToString(), System.Text.Encoding.UTF8, "application/json");
                // Bakiye kısmı
                var getWallet = httpClient.PostAsync("api/User/getBalanceOfAddress", contentCheck);
                getWallet.Result.EnsureSuccessStatusCode();
                var res_balance = getWallet.Result.Content.ReadAsStringAsync().Result;
                var balance_of_toAddress = Convert.ToInt32(JsonConvert.DeserializeObject(res_balance));

                var getSettings = httpClient.GetAsync("api/Blockchain/getDifficultyAndminingReward");
                getSettings.Result.EnsureSuccessStatusCode();
                var res = getSettings.Result.Content.ReadAsStringAsync().Result;
                dynamic j_res = JsonConvert.DeserializeObject(res);
                dynamic miningReward = Convert.ToInt32(j_res["miningReward"]);


                if ((balance_of_toAddress + miningReward) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                {
                    Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarısız RC sınırı aşıldı!", "danger");
                    return RedirectToAction("PendingTransaction");
                }




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