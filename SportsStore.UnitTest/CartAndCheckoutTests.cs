using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTest
{
    [TestClass]
    public class CartAndCheckoutTests
    {
        [TestMethod] //невозможность оплаты при пустой корзине
        public void Cannot_Checkout_Empty_Cart()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>(); //тут создался объект EmailOrderProcessor
            Cart cart = new Cart(); 
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            //action
            ViewResult result = target.Checkout(cart, shippingDetails);

            //assert --заказ не был отправлен по почте, метод ProcessOrder не вызывался
            //отправка 
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            Assert.AreEqual("", result.ViewName); //стандартное представление View(shippingDetails) имеет имя "" (см.СartController)
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid); //т.к. поля модели не заполнены, тест пройдет
        }

        [TestMethod] //impossible checkouting with epmty field
        public void Cannot_Checkout_Invalid_ShippingDetails() {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Product { Name = "Pen" }, 1);

            ShippingDetails shippingDetails = new ShippingDetails();
            //shippingDetails.Line1 = "Test1";

            CartController target = new CartController(null, mock.Object);

            //adding mistakes in model
            target.ModelState.AddModelError("error", "error");

            ViewResult result= target.Checkout(cart, shippingDetails);

            //assert,that next method never calls with any value of parameters
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            //assert, that returns standart view
            Assert.AreEqual("", result.ViewName);

            //assert,that view gets invalid model
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order() {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(),1);

            CartController target = new CartController(null,mock.Object);
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //assert

            //метод ProcessorOrder вызывается
            mock.Verify(m=>m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);

            //метод возвращает представление ""Сomplited
            Assert.AreEqual("Completed", result.ViewName);

            //представлению передается допустимая модель
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
