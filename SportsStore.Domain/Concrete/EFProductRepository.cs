using System;
using System.Collections.Generic;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Linq;

namespace SportsStore.Domain.Concrete
{
    //класс, который привяжем к интерфейсу--его экземпляр будет создаваться Ninject
    public class EFProductRepository:IProductRepository
    {
        private EFDbContext context = new EFDbContext();

        //READ
        public IEnumerable<Product> Products {
            get { return context.Products; } //возвращаем таблицу Products из базы данных
        }

        //UPDATE AND CREATE
        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0) {
                context.Products.Add(product);
            }
            else {
                Product dbEntry = context.Products.Find(product.ProductID);
                if (dbEntry != null) {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                    dbEntry.ImageData = product.ImageData;
                    dbEntry.ImageMimeType = product.ImageMimeType;
                }
            }
            context.SaveChanges();
        }

        //DELETE
        public Product DeleteProduct(int productID)
        {
            //Product dbEntry = context.Products.Find(productID);
            Product dbEntry = context.Products
                              .FirstOrDefault(p => p.ProductID == productID);

            if (dbEntry != null){
                context.Products.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}
