using SportsStore.Domain.Entities;
using System.Data.Entity;

namespace SportsStore.Domain.Concrete
{
    public class EFDbContext:DbContext
    {
        //указываем Entity, что представлять нужно параметр (модель) типа Product--связь между БД и классом Product
        //затем настраиваем корневой Web.config
        //затем создаем класс, создающий контекст и извлекающий из него таблицу Products
        //затем этот класс суем в Niject
        public DbSet<Product> Products { get; set; } 
    }
}
