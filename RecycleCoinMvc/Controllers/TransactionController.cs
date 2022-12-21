using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;
using RecycleCoin.Entities.Concrete;

namespace RecycleCoinMvc.Controllers
{
    public class TransactionController : Controller
    {
        private BlockchainSchema _blockchainSchema;

        public TransactionController()
        {
            _blockchainSchema = new BlockchainSchema();
        }

        // GET: Transaction
        [Authorize]
        public ActionResult CreateTransaction()
        {
            
            var user = new UserManager<AppUser>(new UserStore<AppUser>(new RecycleCoinDbContext())).FindByIdAsync(User.Identity.GetUserId()).Result;
            if (Request.HttpMethod == "POST")
            {
                try
                {
                    _blockchainSchema = _blockchainSchema.GetBlockchain();
                    var fromAddress = user.PrivateKey;
                    var toAddress = Request.Form["toAddress"];

                    if (toAddress.Equals(user.PublicKey))
                    {
                        Session["toast"] = new Toastr("İşlem", "İşleminiz Başarısız! göndermek istediğiniz adres sizin adresiniz!", "danger");
                        return View();
                    }
                    var balanceOfToAddress = _blockchainSchema.GetBalanceOfAddress(toAddress);
                    var amount = Request.Form["amount"];

                    if ((balanceOfToAddress + Convert.ToInt32(amount)) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                    {
                        Session["toast"] = new Toastr("İşlem", "İşleminiz başarısız RC sınırı aşıldı!", "danger");
                        return View();
                    }

                    _blockchainSchema.AddTransaction(fromAddress, toAddress, amount);
                    Session["toast"] = new Toastr("İşlem", "İşleminiz başarıyla gerçekleştirildi", "success");
                }
                catch (Exception)
                {
                    Session["toast"] = new Toastr("İşlem", "İşleminiz Başarısız!", "danger");
                }

            }

            
            return View(user);
        }

        public ActionResult PendingTransaction()
        {
            try
            {
                _blockchainSchema = _blockchainSchema.GetBlockchain();
                var pendingTransactions = _blockchainSchema.pendingTransactions;
                return View(pendingTransactions);
            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("İşlem", "Bekleyen işlemleri görüntülerken bir hata oluştu! Lütfen daha sonra tekrar deneyin", "danger");
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        public ActionResult MinePendingTransaction()
        {
            try
            {
                var user = new UserManager<AppUser>(new UserStore<AppUser>(new RecycleCoinDbContext())).FindByIdAsync(User.Identity.GetUserId()).Result;
                _blockchainSchema = _blockchainSchema.GetBlockchain();
                var balance_of_toAddress = _blockchainSchema.GetBalanceOfAddress(user.PublicKey);

                var miningReward = _blockchainSchema.miningReward;
                
                if ((balance_of_toAddress + miningReward) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                {
                    Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarısız RC sınırı aşıldı!", "danger");
                    return RedirectToAction("PendingTransaction");
                }

                _blockchainSchema.MinePendingTransactions(user.PublicKey);

                Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarıyla gerçekleştirildi.", "success");

            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarısız.", "danger");
                return RedirectToAction("PendingTransaction");
            }



            return RedirectToAction("Index", "Home");
        }

    }
}