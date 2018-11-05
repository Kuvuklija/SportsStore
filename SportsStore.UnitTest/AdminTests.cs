using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTest
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product{ ProductID=1, Name="P1"},
            new Product{ ProductID=2, Name="P2"},
            new Product{ ProductID=3, Name="P3"}
            });

            AdminController target = new AdminController(mock.Object);

            //action
            IEnumerable<Product> result = (IEnumerable<Product>)target.Index().ViewData.Model;
            Product[] resultToArray = result.ToArray();

            //assert
            Assert.AreEqual(3, resultToArray.Length);
            Assert.AreEqual("P1", resultToArray[0].Name);
            Assert.AreEqual("P2", resultToArray[1].Name);
            Assert.AreEqual("P3", resultToArray[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product() {

            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m=>m.Products).Returns(new Product[] {
                new Product{ ProductID=1, Name="P1"},
                new Product{ ProductID=2, Name="P2"},
                new Product{ ProductID=3, Name="P3"}
            });

            AdminController target = new AdminController(mock.Object);
            
            //action
            Product p1=(Product)target.Edit(1).ViewData.Model;
            Product p2=target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            //assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[]{
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"},
                new Product{ProductID=3,Name="P3"}
            });

            AdminController target = new AdminController(mock.Object);

            //action
            Product p4=target.Edit(4).ViewData.Model as Product;

            //assert
            Assert.IsNull(p4);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            //action ---> try to save the product
            ActionResult result = target.Edit(product);
            //assert
            mock.Verify(m => m.SaveProduct(product)); //method is calling
            Assert.IsInstanceOfType(result,typeof(ActionResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error"); //add mistake
            //action
            ActionResult result = target.Edit(product);
            //assert
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never); //or product
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Product() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1, Name="P1"},
                new Product{ProductID=2, Name="P2"},
                new Product{ProductID=3, Name="P3"}
            });
            AdminController target = new AdminController(mock.Object);

            //action
            target.Delete(2);

            //assert
            mock.Verify(m => m.DeleteProduct(2));
        }

        [TestMethod]
        public void Can_Login_With_Valid_Credentials() {
            //arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            //если б тут статич метод был, мы бы не смогли задать поведение функции (по сути, мы ведь не знаем реальн. пароля)
            //смысл мока в том, что только при таких user/password функция вернет true
            mock.Setup(m => m.Authentificate("admin", "secret")).Returns(true); 

            LoginViewModel model = new LoginViewModel { UserName="admin1", Password="secret"};

            AccountController target = new AccountController(mock.Object);

            //action
            ActionResult result = target.Login(model, "/MyUrl");

            //assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credintials() {
            //arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authentificate("badUser", "badPassword")).Returns(false);
            AccountController target = new AccountController(mock.Object);

            LoginViewModel model = new LoginViewModel { UserName = "badUser", Password="badPassword" };

            //action
            ActionResult result = target.Login(model, "/MyUrl");

            //assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Retrive_Image_Data() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[] {
                new Product{ProductID=2,ImageData=new byte[]{ },ImageMimeType="image/png"},
                new Product{ProductID=1},
                new Product{ProductID=3}
            });
            ProductController target = new ProductController(mock.Object);

            //action
            FileContentResult result = target.GetImage(2);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual("image/png", result.ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[] {
                new Product{ProductID=1},
                new Product{ProductID=2}
            });
            ProductController target = new ProductController(mock.Object);

            //action
            FileContentResult result = target.GetImage(20);

            //assert
            Assert.IsNull(result);

        }
    }
}
