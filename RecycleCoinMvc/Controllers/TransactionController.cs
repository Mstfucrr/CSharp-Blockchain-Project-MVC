using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecycleCoinMvc.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult CreateTransaction()
        {
            return View();
        }

        public ActionResult PendingTransaction()
        {
            return View();
        }

    }
}