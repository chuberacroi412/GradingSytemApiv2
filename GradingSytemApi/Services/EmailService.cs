using GradingSytemApi.Common.Helpers;
using GradingSytemApi.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GradingSytemApi.Services
{
    public class EmailService
    {
        private readonly UserManager<Account> _userManager;

        public EmailService(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        private async Task SendEmail(string toEmail, string toName, string subject, string body)
        {
            toEmail = "lamanhtuan1299@gmail.com";
            using(MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(Settings.SEND_EMAIL_ADDRESS, Settings.SEND_EMAIL_NAME);
                mail.To.Add(new MailAddress(toEmail, toName));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(Settings.SEND_EMAIL_SMTP_ADDRESS, 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(Settings.SEND_EMAIL_ADDRESS, Settings.SEND_EMAIL_PASSWORD);

                    await smtp.SendMailAsync(mail);
                }
            }
        }

        private bool IsValidEmail(string mailAddress)
        {
            try
            {
                var mail = new MailAddress(mailAddress);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task SendAccountInformation(string userId, string password)
        {
            Account user = await _userManager.FindByIdAsync(userId);
            string fullName = $"{user.FirstName} {user.LastName}";
            string subject = "Account information";
            string body = $"Bienvenue {fullName} <br/> Your password is {password}";

            if(true || IsValidEmail(user.Email))
            {
                await SendEmail(user.Email, fullName, subject, body);
            }
        }
    }
}
