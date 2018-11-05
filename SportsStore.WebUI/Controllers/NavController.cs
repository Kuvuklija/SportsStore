using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repositoryParam)
        {
            repository = repositoryParam;
        }

        //панель категорий
        //метод работает с частичным представлением и выбирает все различающиеся категории
        //paramHorizontalLayout --при сжатии браузера из Layout 
        public PartialViewResult Menu(string category=null)//, bool paramHorizontalLayout=false) 
        {
            //1 вариант---> пример использования ViewBag для подсветки кнопки в MenuHorizontal
            ViewBag.SelectedCategory = category;

            //2 вариант---> подсветка через модель представления в Menu
            SelectedCategoryModel modelCategory = new SelectedCategoryModel()
            {
                selectedCategory = category,
                categories = repository.Products
                    .Select(x => x.Category)
                    .Distinct()
                    .OrderBy(y => y)
            };

            //string viewName = paramHorizontalLayout ? "MenuHorizontal" : "Menu";--->после создания FlexMenu не нужно

            //return PartialView(viewName, modelCategory);
            return PartialView("FlexMenu", modelCategory);

            //относилось к 1 варианту---> пример использования ViewBag для подсветки кнопки
            //IEnumerable<string> categories = repository.Products
            //    .Select(x => x.Category)
            //    .Distinct()
            //    .OrderBy(y => y); //!--тут упорядочиваем уже по отобранным категориям, т.е. у=x.Category

            //return PartialView(categories);
        }
    }
}