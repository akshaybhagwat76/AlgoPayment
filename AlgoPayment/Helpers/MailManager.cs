using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace AlgoPayment.Helpers
{
    public class MailManager
    {
        public string SendMail(string to, string subject, string data)
        {
            try
            {
               
                MailMessage mm = new MailMessage();


                mm.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"]);

                mm.Subject = subject;

                mm.To.Add(to);

                mm.Body = data;
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;

                smtp.Send(mm);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}