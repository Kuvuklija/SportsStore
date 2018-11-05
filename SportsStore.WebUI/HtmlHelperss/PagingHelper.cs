using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.HtmlHelperss 
{
    //содержит расширяющий метод PagingHelper (расширяет системный HtmlHelper), генерирующий разметку для ссылок на страницы сайта
    public static class PagingHelper 
    {
        //!!! метод PageLinks будет вызываться из представления List (ну и в тесте)

        public static MvcHtmlString PageLinks(this HtmlHelper html, //этот парамет при вызове опускаем
            PageInfo pagingInfo,
            Func<int,string> pageUrl) //ссылка-делегат,указывающий на метод, описанный в UnitTest
        {
            StringBuilder result = new StringBuilder();
            for(int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i)); //вызов метода делегата, описанного в UnitTest
                tag.InnerHtml = i.ToString(); //значение элемента (выводится на страницу), т.е.номера страниц в разметке...>1<... это строки
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected"); //выделяем номер текущей страницы
                    tag.AddCssClass("btn-primary");
                }
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}