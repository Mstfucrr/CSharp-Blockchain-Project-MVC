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
    public class UserController : Controller
    {

        private readonly HttpClient httpClient;


        public UserController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
        }


        // GET: User
        public ActionResult Index()
        {
            Console.WriteLine(Session["User"]);
            ViewData["User"] = Session["User"];
            return View();
        }

        public ActionResult Wallet(string address)
        {
            
            if (Request.HttpMethod == "POST")
            {
                address = Request.Form["address"];
            }
            var content = new StringContent(new JObject(new JProperty("Address", address)).ToString(), System.Text.Encoding.UTF8,"application/json");
            // Bakiye kısmı
            var getWallet = httpClient.PostAsync("api/User/getBalanceOfAddress", content);
            getWallet.Result.EnsureSuccessStatusCode();
            var res_balance = getWallet.Result.Content.ReadAsStringAsync().Result;
            var balance_j_res = JsonConvert.DeserializeObject(res_balance);
            // İşlemlerin çekilceği kısım
            var contentx = new StringContent(new JObject(new JProperty("Address", address)).ToString(), System.Text.Encoding.UTF8, "application/json");

            var getTransaction = httpClient.PostAsync("api/User/getTransactionsOfAddress", contentx);
            getTransaction.Result.EnsureSuccessStatusCode();
            var res_transactions = getTransaction.Result.Content.ReadAsStringAsync().Result;
            var res_transactions_j = JsonConvert.DeserializeObject(res_transactions);

            ViewBag.address = address;
            ViewBag.balance = balance_j_res;
            ViewBag.transactions = res_transactions_j;
            return View();
        }

        public ActionResult Login()
        {
            var method = Request.HttpMethod;
            if (method == "POST")
            {
                return RedirectToAction("Index");
            }
            Session.Clear();
            return View();



        }

        public ActionResult Register()
        {
            if (Request.HttpMethod == "POST")
            {
                var nameValueCollection = Request.Form;
                var user = new User
                {
                    Name = nameValueCollection["regName"],
                    Lastname = nameValueCollection["regLastname"],
                    Username = nameValueCollection["regUsername"],
                    Password = nameValueCollection["regPassword"]
                };

                return RedirectToAction("Index");
            }
            
            return View();

        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }
    }
}