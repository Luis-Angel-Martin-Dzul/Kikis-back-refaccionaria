using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceEMail : IServiceEMail {

        private readonly EmailSettings _emailSettings;
        public ServiceEMail(IOptions<EmailSettings> emailSettings) {
            _emailSettings = emailSettings.Value;
        }

        public bool SendUserPasswordEmail(string to, string password) {
            try {
                var mailMessage = new MailMessage(
                    _emailSettings.FromEmail,
                    to,
                    "Credenciales de Acceso - Refaccionaria Kikis",
                    $"<p>Se ha creado una cuenta para usted. A continuación encontrará su contraseña de acceso:</p><div style='margin: 20px 0;'><strong style='font-size: 18px;'>Contraseña: {password}</strong></div><p>Le recomendamos cambiarla después de su primer ingreso.</p><p>¡Gracias por confiar en nosotros!</p>"
                ) {
                    IsBodyHtml = true
                };

                var client = new SmtpClient(_emailSettings.SmtpClient) {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password)
                };

                client.Send(mailMessage);
                client.Dispose();
            }
            catch(Exception ex) {
                throw new Exception("Error al enviar el correo: " + ex.Message);
            }

            return true;
        }

    }
}
