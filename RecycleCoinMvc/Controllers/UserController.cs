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

        public ActionResult Wallet()
        {
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