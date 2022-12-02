using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using RecycleCoin.Business.Concrete;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;
using RecycleCoin.Entities.Concrete;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly UserRecycleItemManager _userRecycleItemManager;
        private readonly BlockchainApi _blockchainApi;


        public UserController()
        {

            var userStore = new UserStore<AppUser>(new RecycleCoinDbContext());
            _userManager = new UserManager<AppUser>(userStore);
            _userRecycleItemManager = new UserRecycleItemManager(new EfUserRecycleItemDal());
            _blockchainApi = new BlockchainApi();
        }


        // GET: User
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Wallet()
        {

            var user = HttpContext.User.Identity.IsAuthenticated 
                ? _userManager.FindByIdAsync(HttpContext.User.Identity.GetUserId()).Result 
                : null;
            var address = user != null ? user.PublicKey : "";

            if (Request.HttpMethod == "POST")
            {
                address = Request.Form["address"];
                user = _userManager.Users.FirstOrDefault(u=>u.PublicKey == address);


            }
            var balance_j_res = _blockchainApi.Post("api/User/getBalanceOfAddress", new List<JProperty> { new("Address", address) });
            // İşlemlerin çekilceği kısım
            var res_transactions_j = _blockchainApi.Post("api/User/getTransactionsOfAddress", new List<JProperty> { new("Address", address) });


            ViewBag.address = address;
            ViewBag.balance = balance_j_res;

            ViewBag.carbon = user?.Carbon ?? 0;
            ViewBag.username = user?.UserName ?? "Bulunamadı";
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
                var user = _userManager.Find(userLoginViewModel.Username, userLoginViewModel.Password);

                if (user == null)
                {
                    Session["toast"] = new Toastr("Giriş Yap", "Yanlış kullanıcı adı veya parola.", "warning");
                }
                else
                {
                    var authManager = HttpContext.GetOwinContext().Authentication; //login işlemini yerine getiren nesne.
                    var identity = _userManager.CreateIdentity(user, "ApplicationCookie"); //cookie oluşturup authManager aracılığıyla kullanıcıya göndericez.

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
                var j_res = _blockchainApi.Get("api/User/generateKeyPair");
                ViewBag.data = j_res;
                user.PublicKey = ViewBag.data["publicKey"];
                user.PrivateKey = ViewBag.data["privateKey"];
                var result = _userManager.Create(user, userRegisterViewModel.Password);

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

            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecycleItems()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var userRecycleItemsById = _userRecycleItemManager.GetListByUserId(userId);
            ViewBag.RecycleItems = userRecycleItemsById;
            return View();

        }
    }
}