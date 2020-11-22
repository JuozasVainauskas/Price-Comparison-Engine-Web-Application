using System;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PCE_Web.Models;

namespace PCE_Web.Classes
{
    internal class EmailSender : IEmailSenderInterface
    {
        private readonly UserOptions _userOptions;
        public readonly IConfiguration Configuration;
        public EmailSender(IConfiguration configuration, IOptions<UserOptions> userOptions)
        {
            Configuration = configuration;
            _userOptions = userOptions.Value;
        }

        [HttpGet]
        public async void SendEmail(string code, string email) 
        { 
            var client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                
                Credentials = new NetworkCredential()
                {
                     UserName = _userOptions.SecretMail,
                     Password = _userOptions.SecretPassword
                }
            };

            var fromEmail = new MailAddress(_userOptions.SecretMail, "Smart Shop");
            var toEmail = new MailAddress(email, "Naudotojas");
            var message = new MailMessage()
            {
                IsBodyHtml = true,
                From = fromEmail,
                Subject = "Email patvirtinimas",
                Body = "Sveiki,<br>kad patvirtintumėte, jog tai yra jūsų email adresas, prašome įvesti šį kodą:<br><br><b>" + code + "</b><br><br>Jei jūs nesinaudojote mūsų paslaugomis ir niekur nesiregistravote, prašome ignoruoti šį laišką.<br><img src=\"https://i.pinimg.com/originals/d4/2a/8c/d42a8c4e83f0fb3750af810be2abbb23.png\" alt =\"SmartShop\" width=\"50\" height=\"50\"><br><i>Pasirašo,<br>Smart Shop komanda.</i>"
            };
            message.To.Add(toEmail);
            client.SendCompleted += ClientSendCompleted;
            await client.SendMailAsync(message);
        }

        [HttpGet]
        public async void AnswerReportMessage(string email, int identifier, string messageText = "")
        {
            var client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential()
                {
                    UserName = _userOptions.SecretMail,
                    Password = _userOptions.SecretPassword
                }
            };

            var fromEmail = new MailAddress(_userOptions.SecretMail, "Smart Shop");
            var toEmail = new MailAddress(email, "Naudotojas");
            if (identifier == 1)
            {
                var message = new MailMessage()
                {
                    IsBodyHtml = true,
                    From = fromEmail,
                    Subject = "Atsakymas į pranešimą",
                    Body = "Sveiki,<br>administratorius perskaitė Jūsų pranešimą ir norėtų tęsti bendradarbiavimą šalinant klaidas/įgyvendinant pasiūlymus. <br><b>Administratoriaus pranešimas:</b> <br><br> " + messageText + " <br><img src=\"https://i.pinimg.com/originals/d4/2a/8c/d42a8c4e83f0fb3750af810be2abbb23.png\" alt =\"SmartShop\" width=\"50\" height=\"50\"><br><i>Pasirašo,<br>Smart Shop komanda.</i>"
                };
                message.To.Add(toEmail);
                client.SendCompleted += ClientSendCompleted;
                await client.SendMailAsync(message);
            }
            else
            {
                var message = new MailMessage()
                {
                    IsBodyHtml = true,
                    From = fromEmail,
                    Subject = "Atsakymas į pranešimą",
                    Body = "Sveiki,<br>dėkojame už pranešimą, paliktą mūsų svetainėje.Jei jūs nesinaudojote mūsų paslaugomis ir nieko nepranešėte, prašome ignoruoti šį laišką.<br><img src=\"https://i.pinimg.com/originals/d4/2a/8c/d42a8c4e83f0fb3750af810be2abbb23.png\" alt =\"SmartShop\" width=\"50\" height=\"50\"><br><i>Pasirašo,<br>Smart Shop komanda.</i>"
                };
                message.To.Add(toEmail);
                client.SendCompleted += ClientSendCompleted;
                await client.SendMailAsync(message);
            }
        }

        private static void ClientSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine(e.Error.Message);
                return;
            }
        }
    }
}
