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
    public class ProductController : Controller
    {
        private IProductRepository repository;
        //to break on the page
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository) {
            repository = productRepository;
        }

        public ViewResult List(string category, int page = 1)
        {
            //return View(repository.Products //передаем IEnumerable в представление(констуктор MVC в представлении указал на модель Product (it needs to be corrected))
            //    .OrderBy(p => p.ProductID)
            //    .Skip((page - 1) * PageSize) //skip all what there is before current page
            //    .Take(PageSize));            //view 4 pages

            //теперь вместо IEnumerable передаем в представление экземпляр ProductListViewModel -- это нужно учесть в тестах

            ProductListViewModel model = new ProductListViewModel()
            {
                products = repository.Products
                          .Where(p => category == null || p.Category == category) //category==null ---> выбрали все товары, без фильтрации
                          .OrderBy(p => p.ProductID)
                          .Skip((page - 1) * PageSize) //skip all what there is before current page
                          .Take(PageSize),             //select n-pages
                pagingInfo = new PageInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    //TotalItems = repository.Products.Count()
                    TotalItems= category==null ?
                                          repository.Products.Count()
                                          :repository.Products.Where(p=>p.Category==category).Count()
                },
                currentCategory = category
            };

            return View(model);
        }

        //прилетаем из Edit.cshtml
        public FileContentResult GetImage(int productId) {
            Product prod = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            if (prod != null) {
                return File(prod.ImageData, prod.ImageMimeType);
            } else {
                return null;
            }
        }
    }
}
