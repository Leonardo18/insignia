using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

namespace Insignia.Painel.Helpers.Email
{
    public class SendMail
    {
        /// <summary>
        /// Método de envio de e-mail do sistema
        /// </summary>
        /// <param name="DestinatarioNome"></param>
        /// <param name="DestinatarioEmail"></param>
        /// <param name="DestinatarioMensagem"></param>
        /// <returns>True se conseguiu enviar o e-mail com sucesso, false caso de algum erro ou o e-mail seja inválido</returns>
        public bool EnviaEmail(string DestinatarioNome, string DestinatarioEmail, string DestinatarioMensagem)
        {
            bool resp = false;
            string body = string.Empty;

            SmtpClient client = new SmtpClient();

            client.Host = ConfigurationManager.AppSettings["EmailHost"];
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailUser"], ConfigurationManager.AppSettings["EmailPass"]);

            //Read template file from the App_Data folder
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(@"~/Helpers/Email/") + "Template.html"))
            {
                body = sr.ReadToEnd();
            }

            MailMessage mail = new MailMessage();

            var templatePasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email");

            mail.Sender = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.To.Add(new MailAddress(DestinatarioEmail, DestinatarioNome));
            mail.Subject = "Contato";
            mail.IsBodyHtml = true;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;
            mail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

            try
            {
                //Thread T1 = new Thread(delegate ()
                //{
                //    client.Send(mail);
                //});

                //T1.Start();

                client.Send(mail);
            }
            catch (Exception ex)
            {
                resp = false;
            }

            return resp;
        }
    }
}