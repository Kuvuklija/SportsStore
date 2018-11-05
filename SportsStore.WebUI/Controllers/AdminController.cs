using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository repository;

        public AdminController(IProductRepository repo) {
            repository = repo;
        }

        public ViewResult Index() {
            return View(repository.Products);
        }

        //READ-->сюда прилетаем из гиперссылки с именем продукта из представления Edit
        public ViewResult Edit(int productId){
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId); //default is a "null"
            return View(product);
        }

        //UPDATE-->сюда прилетаем из кнопки SAVE представления Edit, параметр (объект) прилетает через связыватель MVC
        [HttpPost] 
        public ActionResult Edit(Product product, HttpPostedFileBase image=null) {
            if (ModelState.IsValid) {
                if (image != null) {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Name);//аналог сеанса и ViewBag
                return RedirectToAction("Index");
            }else { //something is wrong with data values
                return View(product); 
            }
        }

        //CREATE
        public ViewResult Create() {
            return View("Edit",new Product());
        }

        //DELETE
        [HttpPost]
        public ActionResult Delete(int productID) {
            Product deleteProduct = repository.DeleteProduct(productID);
            if (deleteProduct != null) {
                TempData["message"] = string.Format("{0} was deleted", deleteProduct.Name);
            }
            return RedirectToAction("Index");
        }
    }
}