using System;
using System.Linq;
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
    public class CartTests
    {
       //тестим модели

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange
            Product p1 = new Product() { ProductID = 1, Name = "P1" };
            Product p2 = new Product() { ProductID = 2, Name = "P2" };
            Cart target = new Cart();

            //action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].product, p1);
            Assert.AreEqual(results[1].product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //arrange
            Product p1 = new Product() { ProductID = 1, Name = "P1" };
            Product p2 = new Product() { ProductID = 2, Name = "P2" };
            Cart target = new Cart();

            //action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.product.ProductID).ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].quantity, 11);
            Assert.AreEqual(results[1].quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //arrange
            Product p1 = new Product() { ProductID = 1, Name = "P1" };
            Product p2 = new Product() { ProductID = 2, Name = "P2" };
            Cart target = new Cart();

            //action
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p1, 10);
            target.RemoveLine(p1);
            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 1);
            Assert.AreEqual(target.Lines.Where(p => p.product == p1).Count(), 0);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //arrange
            Product p1 = new Product() { ProductID = 1, Name = "P1", Price = 100m };
            Product p2 = new Product() { ProductID = 2, Name = "P2", Price = 200m };
            Cart target = new Cart();

            //action
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            decimal sum = target.ComputeTotalValue();

            Assert.AreEqual(sum, 700m);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //arrange
            Product p1 = new Product() { ProductID = 1, Name = "P1", Price = 100m };
            Product p2 = new Product() { ProductID = 2, Name = "P2", Price = 200m };
            Cart target = new Cart();

            //action
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.Clear();

            Assert.AreEqual(target.Lines.Count(), 0);
        }

        //after binding of model--->тестим контроллер
        [TestMethod]
        public void Can_Add_Product_To_Cart()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ ProductID=1, Name="P1", Category="Apples"},
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object,null);

            //act
            target.AddToCart(cart, 1, null);

            //assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_And_Redirect() {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1, Name="Apple"}
            });
               
            Cart cart = new Cart();
            CartController taget = new CartController(mock.Object,null);

            //action- adding product to cart
            RedirectToRouteResult result = taget.AddToCart(cart, 2, "myUrl");

            //assert
            //походу, из контроллера выщемляем--->отсель: return RedirectToAction("Index", new { returnUrl });
            Assert.AreEqual(result.RouteValues["action"], "Index"); 
            Assert.AreEqual(result.RouteValues["returnUrl"],"myUrl");
        }

        //тестим модель представления, передаваемую в Index()
        [TestMethod]
        public void Can_View_Cart_Contents(){

            //arrange
            CartController taget = new CartController(null,null);
            Cart cart = new Cart();

            //action
            CardIndexViewModel modelCard = (CardIndexViewModel)taget.Index(cart, "myUrl").ViewData.Model; //лезем в представление и там в модель

            //assert
            Assert.AreEqual(modelCard.Cart, cart);
            Assert.AreEqual(modelCard.ReturnUrl, "myUrl");
        }
    }
}
