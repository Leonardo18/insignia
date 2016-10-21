using System.Net.Mail;

namespace Insignia.Painel.Helpers.CustomAttributes
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

            client.Host = "smtp.live.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("leo_dutra18@hotmail.com", "leo180196180196");

            MailMessage mail = new MailMessage();            

            mail.Sender = new MailAddress("leo_dutra18@hotmail.com", "ENVIADOR");
            mail.From = new MailAddress("leo_dutra18@hotmail.com", "ENVIADOR");
            mail.To.Add(new MailAddress("leo_dutra18@hotmail.com", "RECEBEDOR"));
            mail.Subject = "Contato";
            mail.Body = " Mensagem do site:<br/> Nome:  " + DestinatarioNome + "<br/> Email : " + DestinatarioEmail + " <br/> Mensagem : " + DestinatarioMensagem;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);

                resp = true;
            }
            catch (System.Exception ex)
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