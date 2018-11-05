using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Models
{
    public class SelectedCategoryModel
    {
        public string selectedCategory { get; set; } //выбранная категория, которую мы подсветим
        public IEnumerable<string> categories { get; set; }
    }
}