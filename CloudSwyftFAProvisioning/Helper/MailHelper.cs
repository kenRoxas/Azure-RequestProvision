using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Helper
{
    public class MailHelper
    {
        private static string smtpPass = ConfigurationManager.AppSettings["smtpPass"];
        private static string smtpUser = ConfigurationManager.AppSettings["smtpUser"];
        private static string smtpSender = ConfigurationManager.AppSettings["smtpSender"];
        private static string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
        public static void SendMail(MailInfo model)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(model.SendTo));
            mailMsg.From = new MailAddress(smtpSender, "CloudSwyft Global Systems Inc");
            mailMsg.Subject = model.Subject;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.HtmlBody, null, MediaTypeNames.Text.Html));
            SmtpClient smtpClient = new SmtpClient(smtpHost, Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);
        }
    }

    public class MailInfo
    {
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
}
