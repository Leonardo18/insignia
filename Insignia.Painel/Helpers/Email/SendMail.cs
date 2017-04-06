using Insignia.DAO.Util;
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
        private Utilitarios Util = new Utilitarios();

        /// <summary>
        /// Método de envio de e-mail do sistema
        /// </summary>
        /// <param name="DestinatarioNome">Nome do destinatário</param>
        /// <param name="DestinatarioEmail">Email do destinatário</param>
        /// <param name="DestinatarioMensagem">Mensagem ao destinatário</param>
        /// <returns>True se conseguiu enviar o e-mail com sucesso, false caso de algum erro ou o e-mail seja inválido</returns>
        public bool EnviaEmail(string DestinatarioNome, string DestinatarioEmail, string DestinatarioMensagem, string Assunto, string Template, string Token)
        {
            bool resp = false;
            string body = string.Empty;

            SmtpClient client = new SmtpClient();

            client.Host = ConfigurationManager.AppSettings["EmailHost"];
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailUser"], ConfigurationManager.AppSettings["EmailPass"]);

            //Lê o template de e-mail
            using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(@"~/Helpers/Email/") + Template))
            {
                body = sr.ReadToEnd();
            }

            MailMessage mail = new MailMessage();

            var templatePasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email");

            mail.Sender = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailUser"], "Insígnia");
            mail.To.Add(new MailAddress(DestinatarioEmail, DestinatarioNome));
            mail.Subject = Assunto;
            mail.IsBodyHtml = true;
            mail.Body = body.Replace("[Email]", Util.Criptografar(DestinatarioEmail)).Replace("[Token]", Token);
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;
            mail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

            try
            {
                Thread EnviaEmail = new Thread(delegate ()
                {
                    client.Send(mail);
                });

                EnviaEmail.Start();

                resp = true;
            }
            catch (Exception)
            {
                resp = false;
            }

            return resp;
        }
    }
}