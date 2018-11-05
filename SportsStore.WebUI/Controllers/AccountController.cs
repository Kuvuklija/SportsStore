using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        IAuthProvider authProvider;

        public AccountController(IAuthProvider auth) {
            authProvider = auth;
        }

        //метод по умолчанию в который передаются поля из модели представления
        public ViewResult Login() {
            return View();
        }

        //после заполнения полей и попытки залогиниться---> интересно, что в returnUrl находится /Admin/Index, т.е. страница, затребовавшая авторизацию
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl) {
            if (ModelState.IsValid) {
                if (authProvider.Authentificate(model.UserName, model.Password)) {
                    return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                } else {
                    ModelState.AddModelError("","Incorrect username or password!");
                    return View();
                }
            } else {
                return View();
            }
        }
    }
}