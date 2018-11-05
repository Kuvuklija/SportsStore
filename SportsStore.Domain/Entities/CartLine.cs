using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    //товар в корзине
    public class CartLine{
        public Product product { get; set; }
        public int quantity { get; set; }
    }

    //корзина
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        //добавляем в корзину
        public void AddItem(Product selectedProduct, int selectedQuantity){
            CartLine line = lineCollection
                          .Where(p => p.product.ProductID == selectedProduct.ProductID)
                          .FirstOrDefault(); //??? what is default

            if (line == null){
                lineCollection.Add(new CartLine { product = selectedProduct, quantity = selectedQuantity });
            }else{
                line.quantity +=selectedQuantity;
            }
        }

        //удаляем из корзины
        public void RemoveLine(Product deletedProduct) {
            lineCollection.RemoveAll(r=>r.product.ProductID==deletedProduct.ProductID);
        }

        //общая сумма покупок
        public decimal ComputeTotalValue() {
            return lineCollection.Sum(s => s.product.Price * s.quantity);
        }

        //очистить корзину
        public void Clear() {
            lineCollection.Clear();
        }

        //вернуть список товаров из корзины
        public IEnumerable<CartLine> Lines {
            get { return lineCollection; }
        }
    }
}
