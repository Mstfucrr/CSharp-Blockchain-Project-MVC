using RecycleCoin.Business.Concrete;
using RecycleCoin.DataAccess.Concrete.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RecycleCoin.Entities.Concrete;
using RecycleCoinMvc.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using RecycleCoin.DataAccess.Concrete.EntityFramework.Contexts;

namespace RecycleCoinMvc.Areas.Admin.Controllers
{
    public class RecycleCenterController : Controller
    {
        // GET: Admin/RecycleCenter
        private readonly CategoryManager _categoryManager;
        private readonly ProductManager _productManager;
        private readonly UserManager<AppUser> _userManager;
        private Toastr toastr;

        public RecycleCenterController()
        {
            _categoryManager = new CategoryManager(new EfCategoryDal());
            _productManager = new ProductManager(new EfProductDal());
            var userStore = new UserStore<AppUser>(new RecycleCoinDbContext());
            _userManager = new UserManager<AppUser>(userStore);

        }
        public ActionResult Index()
        {
            ViewBag.categories = _categoryManager.GetList();
            Session["toast"] = toastr;
            return View();
        }


        public ActionResult GetProductsByCategory(int categoryId)
        {
            Session["products"] = _productManager.GetListByCategory(categoryId);
            Session["categoryId"] = categoryId;

            return RedirectToAction("Index");
            //return productManager.GetListByCategory(categoryId);
        }

        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = _productManager.GetById(productId);
            var list = Session["cartItems"] as List<CartItem>;
            list ??= new List<CartItem>();
            list.Add(new CartItem(product, quantity));
            Session["cartItems"] = list;
            return RedirectToAction("Index");
            //return productManager.GetListByCategory(categoryId);
        }

        public ActionResult RemoveFromCart(int productId, int amount)
        {
            var cartItems = Session["cartItems"] as List<CartItem>;

            foreach (var cartItem in cartItems)
            {
                if (cartItem.product.Id == productId && cartItem.amount == amount)
                {
                    cartItems.Remove(cartItem);
                    break;
                }
            }

            Session["cartItems"] = cartItems;

            return RedirectToAction("Index");
        }

        public ActionResult RecyleItems(string toAddress, int totalCarbon)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PublicKey == toAddress);
            user.Carbon += totalCarbon;
            _userManager.Update(user);
            Session.Remove("cartItems");

            return RedirectToAction("Index");
        }

    }
}