using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinding:IModelBinder{

        private const string sessionKey = "Cart";
        
        //Связыватель. Метод реализует инерфейс IModelBinders
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            //получить объект Cart из сеанса
            Cart cart = null;
            if (controllerContext.HttpContext.Session != null) {
                cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            }
            //создать экземпляр Cart, если он не обнаружен в данных сеанса
            if (cart == null) {
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null)
                    controllerContext.HttpContext.Session[sessionKey] = cart;
            }
            //возвратить объект Cart
            return cart;
        }
    }
}