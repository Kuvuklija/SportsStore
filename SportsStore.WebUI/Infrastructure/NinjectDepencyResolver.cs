using System;
using System.Collections.Generic;
using System.Web.Mvc; // interface "IDependencyResolver" is dependencing from it
using Ninject;
using System.Linq;
using Moq;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using System.Configuration;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDepencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        //main constructor is calling from NinjectWebCommon
        public NinjectDepencyResolver(IKernel kernelParam) {
            kernel = kernelParam;
            AddBindings();
        }

        private void AddBindings()
        {
            //there are binds here
            //imitation realisation of repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> {
                 new Product{Name="Football",Price=25},
                 new Product{Name="Surf board",Price=179},
                 new Product{Name="Running shoes",Price=95}
            });
            //return the same imitation (Moq) object
            //kernel.Bind<IProductRepository>().ToConstant(mock.Object);

            //real db object
            kernel.Bind<IProductRepository>().To<EFProductRepository>();

            //будем внедрять как параметр метода: EmailOrderProcessor(EmailSettings settings)
            EmailSettings emailSettings = new EmailSettings{
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            };

            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);

            //authentification
            kernel.Bind<IAuthProvider>().To<FormsAuthProvider>();

        }

        //realization of interface
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}