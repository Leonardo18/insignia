using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
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
            bool resp;

            SmtpClient client = new SmtpClient();

            client.Host = ConfigurationManager.AppSettings["EmailHost"];
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailUser"], ConfigurationManager.AppSettings["EmailPass"]);

            string body;
            //Read template file from the App_Data folder
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(@"~/Helpers/Email/") + "Template.cshtml"))
            {
                body = sr.ReadToEnd();
            }

            MailMessage mail = new MailMessage();

            var templatePasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email");

            mail.Sender = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.To.Add(new MailAddress(DestinatarioEmail, DestinatarioNome));
            mail.Subject = "Contato";
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);

                resp = true;
            }
            catch (Exception ex)
            {
                resp = false;
            }
            finally
            {
                mail = null;
            }

            return resp;
        }
    }
}