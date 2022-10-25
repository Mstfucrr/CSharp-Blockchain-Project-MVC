using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;
using RecycleCoin.Business.Concrete;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using RecycleCoin.Entities.Concrete;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    public class SettingsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly CategoryManager categoryManager;
        private readonly ProductManager productManager;

        public SettingsController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
            categoryManager = new CategoryManager(new EfCategoryDal());
            productManager = new ProductManager(new EfProductDal());
        }



        // GET: Admin/Settings
        public ActionResult Index()
        {

            if (Request.HttpMethod == "POST")
            {
                try
                {
                    var SettingsContent = new JObject(
                        new JProperty("difficulty", Request.Form["difficulty"]),
                        new JProperty("reward", Request.Form["reward"])).ToString();

                    var s_content = new StringContent(SettingsContent, System.Text.Encoding.UTF8, "application/json");


                    var setSettings = httpClient.PostAsync("api/Blockchain/setDifficultyAndminingReward", s_content);
                    setSettings.Result.EnsureSuccessStatusCode();
                    Session["toast"] = new Toastr("Ayarlar", "Ayarlama işlemi başarıyla gerçekleştirildi", "success");

                }
                catch (Exception e)
                {
                    Session["toast"] = new Toastr("Ayarlar", "Ayarlama işlemi Başarısız !", "danger");

                }

            }


            var getSettings = httpClient.GetAsync("api/Blockchain/getDifficultyAndminingReward");
            getSettings.Result.EnsureSuccessStatusCode();
            var res = getSettings.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            ViewBag.settings = j_res;
            ViewBag.categories = categoryManager.GetList();

            return View();
        }


        public ActionResult AddCategory()
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


            return RedirectToAction("Index");
        }

        public ActionResult AddProduct()
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