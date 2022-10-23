using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using RecycleCoinMvc.Models;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    public class SettingsController : Controller
    {
        private readonly HttpClient httpClient;


        public SettingsController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
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
                    ViewBag.toast = new Toastr("Ayarlar", "Ayarlama işlemi başarıyla gerçekleştirildi", "success");

                }
                catch (Exception e)
                {
                    ViewBag.toast = new Toastr("Ayarlar", "Ayarlama işlemi Başarısız !", "danger");

                }

            }


            var getSettings = httpClient.GetAsync("api/Blockchain/getDifficultyAndminingReward");
            getSettings.Result.EnsureSuccessStatusCode();
            var res = getSettings.Result.Content.ReadAsStringAsync().Result;
            var j_res = JsonConvert.DeserializeObject(res);
            ViewBag.settings = j_res;
            
            return View();
        }
    }
}