using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;

        public CartController(IProductRepository repo,IOrderProcessor proc) {
            repository = repo;
            orderProcessor = proc;
        }

        //these method called by Html.BeginForm("AddToCart", "Cart") from ProductSummary
        public RedirectToRouteResult AddToCart(Cart cart,int ProductID, string returnUrl) 
            //Cart car--- появился после связывания модели--->MVC видит его ищет в связывателях (Global.asax). Найдя, передает сюда экземпляр Cart
        {
            Product product = repository.Products
                                      .FirstOrDefault(p => p.ProductID == ProductID);

            if (product != null) {
                //GetCart().AddItem(product, 1);
                cart.AddItem(product, 1); //---после связывания модели
            }
            return RedirectToAction("Index", new { returnUrl }); //returnUrl---> Имя как в @Html.Hidden("returnUrl"...
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart,int productGuid,string returnUrlka) {//Cart car--- появился после связывания модели
            Product product = repository.Products
                           .FirstOrDefault(p => p.ProductID == productGuid);
            if (product != null) {
                //GetCart().RemoveLine(product);
                cart.RemoveLine(product); //---после связывания модели

            }
            return RedirectToAction("Index", new { returnUrlka }); //здесь может не биться c именем параметра Index
        }

        //после связывания модели---до этого Cart нельзя было передать в AddToCart и протестить
        //private Cart GetCart() {
        //    Cart cart = (Cart)Session["Cart"]; //извлекаем из сессии корзину по индексу "Cart"--name is random
        //    if (cart == null) {
        //        cart = new Cart();
        //        Session["Cart"] = cart; //assign value to session by index
        //    }
        //    return cart;
        //}

        //method whose is a goal of redirecting
        //public ViewResult Index(string returnUrl) {  //returnUrl---> Имя как в @Html.Hidden("returnUrl"...
        //---> НЕТ! Только productGuid из Html.Hidden("productGuid"..., должен биться с параметром метода RedirectToRouteResult RemoveFromCart(Cart cart,int productGuid...
        //после связывания модели
        public ViewResult Index(Cart cart, string returnUrl) { 
            return View(new CardIndexViewModel
            {
                //cart=GetCart(),
                Cart=cart,
                ReturnUrl= returnUrl
            });
        }

        //сюда приходим при нажатии на виджет корзины в Layout-->вставляем в Layout частичное представление
        public PartialViewResult ViewPartCart(Cart cart) {
            return PartialView(cart);
        }

        //нажатие на кнопку Checkout в корзине
        public ViewResult Checkout() {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart,ShippingDetails shippingDetails) { //параметры - один через связыватель, другой- из представления АВТОМАТОМ, becouse the fields have same names!
            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Sorry,your cart is empty!");
            }
            //ModelState - проверяет валидность заполнения полей в форме Checkout благодаря ее атрибутам!
            if (ModelState.IsValid) {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }else {
                return View(shippingDetails); //незаполненная форма с отображением ошибок
            }
        }
    }
}