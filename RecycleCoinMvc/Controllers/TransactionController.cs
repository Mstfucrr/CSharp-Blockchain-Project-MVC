using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Controllers
{
    public class TransactionController : Controller
    {
        private readonly BlockchainApi _blockchainApi;

        public TransactionController()
        {
            _blockchainApi = new BlockchainApi();
        }

        // GET: Transaction
        [Authorize]
        public ActionResult CreateTransaction()
        {
            if (Request.HttpMethod == "POST")
            {
                try
                {
                    var fromAddress = Request.Form["fromAddress"];
                    var toAddress = Request.Form["toAddress"];

                    if (toAddress.Equals((string)Session["publicKey"]))
                    {
                        Session["toast"] = new Toastr("İşlem", "İşleminiz Başarısız! göndermek istediğiniz adres sizin adresiniz!", "danger");
                        return View();
                    }
                    var balanceOfToAddress = Convert.ToInt32(_blockchainApi.Post("api/User/getBalanceOfAddress",
                        new List<JProperty> { new("Address", toAddress) }));
                    var amount = Request.Form["amount"];

                    if ((balanceOfToAddress + Convert.ToInt32(amount)) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                    {
                        Session["toast"] = new Toastr("İşlem", "İşleminiz başarısız RC sınırı aşıldı!", "danger");
                        return View();
                    }

                    var transactionContent = new List<JProperty>
                    {
                        new("fromAddress", fromAddress),
                        new("toAddress", toAddress),
                        new("amount", amount)
                    };
                    _blockchainApi.Post("api/Transaction/AddTransaction", transactionContent);

                    Session["toast"] = new Toastr("İşlem", "İşleminiz başarıyla gerçekleştirildi", "success");
                }
                catch (Exception)
                {
                    Session["toast"] = new Toastr("İşlem", "İşleminiz Başarısız!", "danger");
                }

            }


            return View();
        }

        public ActionResult PendingTransaction()
        {
            ViewBag.PendingTransactions = _blockchainApi.Get("api/Transaction/getPendingTransactions");
            return View();
        }
        [Authorize]
        public ActionResult MinePendingTransaction(string minerRewardAddress)
        {
            try
            {
                dynamic balance_of_toAddress = Convert.ToInt32(
                    _blockchainApi.Post("api/User/getBalanceOfAddress",
                        new List<JProperty> { new("Address", minerRewardAddress) }));

                dynamic miningReward = Convert.ToInt32(
                    _blockchainApi.Get("api/Blockchain/getDifficultyAndminingReward")["miningReward"]);

                Console.WriteLine(balance_of_toAddress + miningReward);

                if ((balance_of_toAddress + miningReward) >= 100000000) //RC miktarı 100M ile sınırlandırıldı.
                {
                    Session["toast"] = new Toastr("Kazı", "Kazı işleminiz başarısız RC sınırı aşıldı!", "danger");
                    return RedirectToAction("PendingTransaction");
                }

                _blockchainApi.Post("api/Transaction/minerPendingTransactions",
                    new List<JProperty> { new("minerRewardAddress", minerRewardAddress) });

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