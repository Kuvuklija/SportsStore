 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; } //возвратит таблицу из контекста БД (строки тип Product, вся таблица IEnumerable)

        void SaveProduct(Product product);

        Product DeleteProduct(int productID);
    }
}
