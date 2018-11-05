using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor:IOrderProcessor{

        private EmailSettings emailSettings;

        //settings-прилетает как аргумент из NinjectDepencyResolver
        public EmailOrderProcessor(EmailSettings settings) {
            emailSettings = settings;
        }

        //cart передается через связыватель модели 
        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using(var smtpClient=new SmtpClient()) {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);
                if (emailSettings.WriteAsFile) { //почту сохраняем в директории для обработки локальным Smtp-сервером для внешних приложений
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                                     .Append("A new order has been submitted")
                                     .Append("---")
                                     .Append("Items:");
                foreach(var line in cart.Lines) {
                    var subtotal = line.product.Price * line.quantity;
                    body.AppendFormat("{0}x{1} (subtotal{2:c})", line.quantity, line.product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1 ?? "")
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("---1")
                    .AppendFormat("Gift wrap:{0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFrom,
                    emailSettings.MailToAdress,
                    "New order submitted!",//тема
                    body.ToString());

                if (emailSettings.WriteAsFile) {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
                                 
            }
        }
    }

    public class EmailSettings{
        public string MailToAdress = "orders@example.com";
        public string MailFrom = "sportsstore@example.com";
        public bool UseSsl = true;
        public string UserName = "MySmtpUsername";
        public string Password = "MySmtpUsername";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"F:\C#\ASP MVC";
    }
}
