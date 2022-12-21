using System;
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
using RecycleCoin.Entities.Dtos;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly UserRecycleItemManager _userRecycleItemManager;
        private readonly BlockchainApi _blockchainApi;
        private BlockchainSchema _blockchainSchema;
        private readonly UserCheckManager _userCheckManager;


        public UserController()
        {

            var userStore = new UserStore<AppUser>(new RecycleCoinDbContext());
            _userManager = new UserManager<AppUser>(userStore);
            _userRecycleItemManager = new UserRecycleItemManager(new EfUserRecycleItemDal());
            _blockchainApi = new BlockchainApi();
            _userCheckManager = new UserCheckManager();
            _blockchainSchema = new BlockchainSchema();
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
                user = _userManager.Users.FirstOrDefault(u => u.PublicKey == address);
                if (user == null)
                {
                    Session["toast"] = new Toastr("Kullanıcı Bilgileri", "Bu adrese ait kullanıcı bulunamadı!", "warning");
                }


            }

            var balance = 0;
            List<TransactionSchema> transactionsOfUser = null;
            try
            {
                _blockchainSchema = _blockchainSchema.GetBlockchain();
                balance = _blockchainSchema.GetBalanceOfAddress(address);
                // İşlemlerin çekilceği kısım
                transactionsOfUser = _blockchainSchema.GetTransactionsOfAddress(address);

            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Kullanıcı Bilgileri", "Kullanıcı bilgilerini çekerken bir hata oluştu! Lütfen daha sonra tekrar deneyin.", "warning");

            }


            ViewBag.address = address;
            ViewBag.balance = balance;

            ViewBag.carbon = user?.Carbon ?? 0;
            ViewBag.username = user?.UserName ?? "Bulunamadı";
            ViewBag.transactions = transactionsOfUser;
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
                var j_res = _blockchainApi.Get("api/User/generateKeyPair");
                ViewBag.data = j_res;
                var user = new AppUser
                {
                    UserName = userRegisterViewModel.Username,
                    Name = userRegisterViewModel.Name,
                    Lastname = userRegisterViewModel.Lastname,
                    Email = userRegisterViewModel.Email,
                    PublicKey = ViewBag.data["publicKey"],
                    PrivateKey = ViewBag.data["privateKey"]
                };
                if (!userRegisterViewModel.IsNotTcPerson) // tc vatandaşı ise
                {
                    if (userRegisterViewModel.TcNo == null || userRegisterViewModel.Year == null)
                    {
                        Session["toast"] = new Toastr("Kayıt Ol", "Tc kimlik numarası ve Doğum yılınız boş olamaz", "danger");
                        return View();

                    }
                    user.TcNo = Convert.ToInt64(userRegisterViewModel.TcNo);
                    user.Year = Convert.ToInt32(userRegisterViewModel.Year);
                    var checkUserModel = new UserValidationDto
                    {
                        Name = user.Name,
                        LastName = user.Lastname,
                        TcNo = user.TcNo,
                        Year = user.Year
                    };

                    if (!_userCheckManager.IsRealPerson(checkUserModel))    // vatandaşlık bilgileri kontrol edilir
                    {
                        Session["toast"] = new Toastr("Kayıt Ol", "Bu bilgilere ait TC vatandaşı bulunamadı", "danger");
                        return View();

                    }
                }

                var result = _userManager.Create(user, userRegisterViewModel.Password);

                if (result.Succeeded)
                {
                    Session["toast"] = new Toastr("Kayıt", "Kayıt İşleminiz başarıyla gerçekleştirildi", "success");

                    return RedirectToAction("Login");

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
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

        [Authorize]
        public ActionResult RecycleItems()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            if (userId != null)
            {
                var userRecycleItemsById = _userRecycleItemManager.GetListByUserId(userId);
                ViewBag.RecycleItems = userRecycleItemsById;
            }

            return View();

        }
    }
}