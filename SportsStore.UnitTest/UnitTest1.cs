using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelperss;

namespace SportsStore.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // 1 вариант разбиения страниц
            //arrange ---> в тестах не через Ninject, а напрямую в констуктор инжектируем
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"},
                new Product{ProductID=3,Name="P3"},
                new Product{ProductID=4,Name="P4"},
                new Product{ProductID=5,Name="P5"}
                });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //action
            //IEnumerable<Product> result = (IEnumerable<Product>)controller.List(2).Model; //модель из представления
            //теперь мы передали в представление объект ProductListViewModel, поэтому ---> перепишем так ***
            ProductListViewModel result=(ProductListViewModel)controller.List(null,2).Model;

            //assert
            //Product[] prodArray = result.ToArray(); //преобразуем коллекцию в массив
            Product[] prodArray = result.products.ToArray(); //----> а тут так ***

            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name , "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        //2 вариант генерим номера страниц (сравниваем результаты с ожидаемой разметкой)
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //arrange
            HtmlHelper myHelper = null;
            PageInfo pagingInfo = new PageInfo() {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10 };
            Func<int, string> pageUrlDelegate = x => "Page" + x; //когда в PagingHelper будет вызываться Func, там вместо х в цикле подставится i
            //action
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                           + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                           + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Paginate_View_Model()
        {
            //arrange --->проверяем, что данные в объекте представления сформировались верно
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"},
                new Product{ProductID=3,Name="P3"},
                new Product{ProductID=4,Name="P4"},
                new Product{ProductID=5,Name="P5"}
                });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //action
            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model; //теперь объект с номерами страниц суём

            //assert
            PageInfo pageInfo = result.pagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage,3);
            Assert.AreEqual(pageInfo.TotalPages,2);
            Assert.AreEqual(pageInfo.TotalItems, 5);
        }

        [TestMethod]
        public void Can_Filter_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1",Category="Cat1"},
                new Product{ProductID=2,Name="P2",Category="Cat2"},
                new Product{ProductID=3,Name="P3",Category="Cat1"},
                new Product{ProductID=4,Name="P4",Category="Cat2"},
                new Product{ProductID=5,Name="P5",Category="Cat3"}
                });
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //action
            ProductListViewModel results = (ProductListViewModel)controller.List("Cat1", 1).Model;
            Product[] productsArray=results.products.ToArray();

            //assert
            Assert.AreEqual(productsArray.Length, 2);
            Assert.IsTrue(productsArray[0].Category == "Cat1"&& productsArray[0].Name=="P1");
            Assert.IsTrue(productsArray[1].Category == "Cat1" && productsArray[1].Name=="P3");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1",Category="Apple"},
                new Product{ProductID=2,Name="P2",Category="Microsoft"},
                new Product{ProductID=3,Name="P3",Category="Apple"},
                new Product{ProductID=4,Name="P4",Category="Linux"},
                new Product{ProductID=5,Name="P5",Category="Apple"}
                });

            NavController navController = new NavController(mock.Object);

            //action
            IEnumerable<string> result = (IEnumerable<string>)navController.Menu().Model; //в представление передавали IEnumerable<string>
            string[] target = result.ToArray();

            //asset
            Assert.AreEqual(target.Count(), 3);
            Assert.AreEqual(target[0], "Apple");
            Assert.AreEqual(target[1], "Linux");
            Assert.AreEqual(target[2], "Microsoft");
        }

            [TestMethod]
            public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1",Category="Apple"},
                new Product{ProductID=2,Name="P2",Category="Microsoft"}
            });

            //arrange
            NavController target = new NavController(mock.Object);
            string categoryToSelect = "Microsoft";
            //action
            //string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            SelectedCategoryModel selCat = (SelectedCategoryModel)target.Menu(categoryToSelect).Model;
            string result = selCat.selectedCategory;
            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product{ProductID=1,Name="P1",Category="Apple"},
                new Product{ProductID=2,Name="P2",Category="Microsoft"},
                new Product{ProductID=3,Name="P3",Category="Apple"},
                new Product{ProductID=4,Name="P4",Category="Macintosh"},
                new Product{ProductID=5,Name="P5",Category="Apple"},
                new Product{ProductID=6,Name="P6",Category="Macintosh"}
            });

            //arrange
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;
            //action
            int res1 = ((ProductListViewModel)target.List("Apple").Model).pagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)target.List("Macintosh").Model).pagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)target.List("Microsoft").Model).pagingInfo.TotalItems;
            int res4 = ((ProductListViewModel)target.List(null).Model).pagingInfo.TotalItems;

            //assert
            Assert.AreEqual(res1, 3);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3,1);
            Assert.AreEqual(res4, 6);
        }
    }
}
