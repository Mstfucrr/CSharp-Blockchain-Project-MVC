using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            Console.WriteLine(Session["User"]);
            ViewData["User"] = Session["User"];
            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            var nameValueCollection = Request.Form; //formdan giriş bilgilerini çeker
            var user = new User().Login(nameValueCollection["username"], nameValueCollection["password"]);
            if (user.Id == 0)
                Console.WriteLine("Giriş Hatası");
            else
                Session.Add("User", user);

            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Register()
        {
            var nameValueCollection = Request.Form;
            var user = new User();
            user.Name = nameValueCollection["Name"];
            user.Password = nameValueCollection["password"];
            user.Email = nameValueCollection["email"];
            user.Lastname = nameValueCollection["Lastname"];
            user.Username = nameValueCollection["username"];



            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }
    }
}