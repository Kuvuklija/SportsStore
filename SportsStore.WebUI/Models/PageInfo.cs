using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Models
{
    public class PageInfo //модель представления, выводящая на страницу (в представление):
    {
        public int TotalItems { get; set; }   //общее кол-во товаров в БД
        public int ItemsPerPage { get; set; } //товаров на странице
        public int CurrentPage { get; set; }  //текущая страница

        public int TotalPages
        {
            //Ceiling- это ближайшее число в результате округления
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}