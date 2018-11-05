using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    public class Product
    {
        //атрибуты для CRUD в контроллере AdminController

        [HiddenInput(DisplayValue =false)] //скрываем Id в представлении Edit при вызове EditorForModel стр.280
        public int ProductID { get; set; }

        [Required(ErrorMessage ="Please enter a product name")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)] //расширяем поле Description
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage ="Please enter a positive price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Please specify a category")]
        public string Category { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}