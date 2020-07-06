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

                mm.To.Add(to);
                mm.From = new MailAddress(ConfigurationManager.AppSettings["EmailId"]);
                mm.Subject = subject;


                mm.Body = data;

                mm.IsBodyHtml = true;
                
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential
                     (ConfigurationManager.AppSettings["EmailId"], ConfigurationManager.AppSettings["EmailPassword"]);
                //Or your Smtp Email ID and Password
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