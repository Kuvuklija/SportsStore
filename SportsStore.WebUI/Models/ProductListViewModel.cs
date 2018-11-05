using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models //вспомогательный для передачи страниц таблицы в метод List и одноименное представление
{
    public class ProductListViewModel
    {
       public IEnumerable<Product> products { get; set; }
       public PageInfo pagingInfo { get; set; }
       public string currentCategory { get; set; }
    }
}