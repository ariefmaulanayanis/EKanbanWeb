using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EKanbanWeb.Helpers
{
    public class MailHelper
    {
        private readonly IHostEnvironment hostEnvironment;
        public string errMessage;
        private LogHelper logHelper;

        public MailHelper(ILogger logger, IHostEnvironment environment)
        {
            hostEnvironment = environment;
            logHelper = new LogHelper(logger);
        }

        public bool SendMail(string mailSubject, string mailTo, string mailBody)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(mailTo);
            mm.Subject = mailSubject;
            mm.Body = mailBody;
            //mm.From = new MailAddress("arief.maulana.yanis@gmail.com");
            mm.From = new MailAddress("dmia.system.a2d@ap.denso.com");
            mm.IsBodyHtml = false;

            //SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            SmtpClient smtp = new SmtpClient("smtp.office365.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            //smtp.Credentials = new System.Net.NetworkCredential("arief.maulana.yanis@gmail.com", "maniez1982");
            smtp.Credentials = new System.Net.NetworkCredential("dmia.system.a2d@ap.denso.com", "*");
            try
            {
                smtp.Send(mm);
            }
            catch(Exception e)
            {
                errMessage = e.Message;
                logHelper.WriteErrorLog(e);
                return false;
            }
            return true;
        }
    }
}
