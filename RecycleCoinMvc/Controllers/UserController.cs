using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;
using RecycleCoin.Entities.Concrete;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient httpClient;

        private UserManager<AppUser> userManager;

        public UserController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");

            var userStore = new UserStore<AppUser>(new RecycleCoinDbContext());
            userManager = new UserManager<AppUser>(userStore);
        }


        // GET: User
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Wallet()
        {
            var address = HttpContext.User.Identity.IsAuthenticated ? userManager.FindByIdAsync(HttpContext.User.Identity.GetUserId()).Result.PublicKey : "" ;
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl; //gitmek istediği url
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginViewModel userLoginViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Find(userLoginViewModel.Username, userLoginViewModel.Password);

                if (user == null)
                {
                    Session["toast"] = new Toastr("Giriş Yap", "Yanlış kullanıcı adı veya parola.", "warning");
                }
                else
                {
                    var authManager = HttpContext.GetOwinContext().Authentication; //login işlemini yerine getiren nesne.
                    var identity = userManager.CreateIdentity(user, "ApplicationCookie"); //cookie oluşturup authManager aracılığıyla kullanıcıya göndericez.

                    var authProperties = new AuthenticationProperties()
                    {
                        IsPersistent = true, //beni hatırla
                    }; //gönderilirken bir kaç özellikle beraber cookie'nin gönderilmesini sağlar.

                    authManager.SignOut(); //kullanıcı varsa sistemde önce sil sisteme dahil et.
                    authManager.SignIn(authProperties, identity);
                    Session["publicKey"] = user.PublicKey;
                    Session["privateKey"] = user.PrivateKey;
                    Session["toast"] = new Toastr("Giriş Yap", "Başarıyla giriş yaptınız.", "success");


                    return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(UserRegisterViewModel userRegisterViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser();
                user.UserName = userRegisterViewModel.Username;
                user.Name = userRegisterViewModel.Name;
                user.Lastname = userRegisterViewModel.Lastname;
                user.Email = userRegisterViewModel.Email;

                var getKeyPair = httpClient.GetAsync("api/User/generateKeyPair");
                getKeyPair.Result.EnsureSuccessStatusCode();
                var res = getKeyPair.Result.Content.ReadAsStringAsync().Result;
                var j_res = JsonConvert.DeserializeObject(res);
                ViewBag.data = j_res;
                user.PublicKey = ViewBag.data["publicKey"];
                user.PrivateKey = ViewBag.data["privateKey"];
                var result = userManager.Create(user, userRegisterViewModel.Password);

                if (result.Succeeded)
                {
                    Session["toast"] = new Toastr("Kayıt", "Kayıt İşleminiz başarıyla gerçekleştirildi", "success");

                    return RedirectToAction("Login");

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;

            authManager.SignOut();
            Session.Remove("publicKey");
            Session.Remove("privateKey");
            Session["toast"] = new Toastr("Çıkış", "Başarıyla çıkış yaptınız.", "success");

            return RedirectToAction("Index","Home");
        }
    }
}