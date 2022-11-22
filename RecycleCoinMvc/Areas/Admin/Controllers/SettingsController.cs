using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;
using RecycleCoin.Business.Concrete;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoin.Entities.Concrete;
using Microsoft.Owin.BuilderProperties;
using System.Collections.Generic;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly CategoryManager categoryManager;
        private readonly ProductManager productManager;
        private readonly BlockchainApi _blockchainApi;

        public SettingsController()
        {
            categoryManager = new CategoryManager(new EfCategoryDal());
            productManager = new ProductManager(new EfProductDal());
            _blockchainApi = new BlockchainApi();
        }



        // GET: Admin/Settings
        public ActionResult Index()
        {

            if (Request.HttpMethod == "POST")
            {
                try
                {
                    var dif = new JProperty("difficulty", Request.Form["difficulty"]);
                    var rew = new JProperty("reward", Request.Form["reward"]);
                    _blockchainApi.Post("api/Blockchain/setDifficultyAndminingReward", new List<JProperty>{dif,rew});
                    Session["toast"] = new Toastr("Ayarlar", "Ayarlama işlemi başarıyla gerçekleştirildi", "success");

                }
                catch (Exception)
                {
                    Session["toast"] = new Toastr("Ayarlar", "Ayarlama işlemi Başarısız !", "danger");

                }

            }

            var getSettings = _blockchainApi.Get("api/Blockchain/getDifficultyAndminingReward");
            ViewBag.settings = getSettings;
            ViewBag.categories = categoryManager.GetList();

            return View();
        }


        public ActionResult AddCategory()
        {
            try
            {
                var category = new Category
                {
                    Name = Request.Form["name"]
                };
                categoryManager.Add(category);
                var uzanti = Path.GetExtension(Request.Files[0].FileName);
                var yol = $"~/image/Image_Category_{category.Id}{uzanti}";

                var ser = Server.MapPath(yol);

                Request.Files[0].SaveAs(ser);

                category.Image = $"Image_Category_{category.Id}{uzanti}";

                categoryManager.Update(category);

                Session["toast"] = new Toastr("Kategori", "Kategoriniz başarıyla eklendi.", "success");
            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Kategori", "Kategoriniz ekleme işlemi başarısız !", "danger");
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddProduct()
        {
            try
            {
                var product = new Product
                {
                    Name = Request.Form["name"],
                    CategoryId = Convert.ToInt32(Request.Form["categoryId"]),
                    Carbon = Convert.ToInt32(Request.Form["carbon"])
                };
                productManager.Add(product);


                var uzanti = Path.GetExtension(Request.Files[0].FileName);
                var yol = $"~/image/Image_Product_{product.Id}{uzanti}";

                var ser = Server.MapPath(yol);

                Request.Files[0].SaveAs(ser);

                product.Image = $"Image_Product_{product.Id}{uzanti}";

                productManager.Update(product);
                Session["toast"] = new Toastr("Ürün", $"{product} ürünü başarıyla eklendi.", "success");

            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Ürün", "Ürün ekleme işlemi başarısız !", "danger");

            }

            return RedirectToAction("Index");

        }


        public ActionResult SetProductCarbon()
        {

            try
            {
                var productId = Convert.ToInt32(Request.Form["productId"]);
                var newCarbon = Convert.ToInt32(Request.Form["carbon"]);
                var product = productManager.GetById(productId);
                product.Carbon = newCarbon;
                productManager.Update(product);
                Session["toast"] = new Toastr("Ürün Ayarları", $"{product.Name} ürününün Carbon tutarı {product.Carbon} miktarına başarıyla güncellendi.", "success");

            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Ürün Ayarları", "Ayarlama işlemi Başarısız !", "danger");
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteProduct(int productId)
        {
            try
            {
                var product = productManager.GetById(productId);
                productManager.Delete(product);
                Session["toast"] = new Toastr("Ürünü Kaldırma", $"{product.Name} ürünü başarıyla kaldırıldı.", "success");

            }
            catch (Exception)
            {
                Session["toast"] = new Toastr("Ürünü Kaldırma", "ürün kaldırılma işlemi başarısız !", "danger");
            }


            return RedirectToAction("Index");
        }

        public ActionResult GetProductsByCategory(int categoryId)
        {
            Session["products"] = productManager.GetListByCategory(categoryId);
            Session["categoryId"] = categoryId;
            return RedirectToAction("Index");
            //return productManager.GetListByCategory(categoryId);
        }


    }
}