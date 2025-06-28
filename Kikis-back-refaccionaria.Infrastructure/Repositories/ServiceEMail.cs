using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
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

        public bool SendCFDI(TbInvoice invoice, SaleRES sale) {
            try {
                //receptor
                string numero_factura = invoice.Id.ToString();
                string fecha_factura = invoice.CreateDate.ToString("yyyy-MM-dd");
                string receptor_nombre = invoice.Name;
                string receptor_rfc = invoice.RFC.ToString();
                string receptor_cfdi = invoice.UseCFDI.ToString();
                string receptor_cfdi_nombre = invoice.UseCFDIName.ToString();
                string receptor_regimen = invoice.TaxRegime.ToString();
                string receptor_regimen_nombre = invoice.TaxRegimeName.ToString();

                //emisor
                string empresa_emisora = "KIKIS REFACCIONARIA";
                string empresa_emisora_rfc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string numero_factura_direccion = "Calle Ficticia 123, COD_POSTAL MERIDA, YUCATAN, MEXICO";

                //productos
                string productos = "";
                foreach(var product in sale.SaleDetails) {

                    productos += $@"
                    <tr>
                        <td>{product.Quantity}</td>
                        <td>{product.Name}</td>
                        <td>${product.PriceUnit}</td>
                        <td>${product.Total}</td>
                    </tr>";
                }
                string venta_subtotal = sale.SubTotal.ToString();
                string venta_iva  = sale.Iva.ToString();
                string venta_total = sale.Total.ToString();


                string htmlBody = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                  <meta charset='UTF-8'>
                  <title>Factura</title>
                  <style>
                    body {{ font-family: Arial, sans-serif; margin: 40px; color: #333; }}
                    .factura {{ border: 1px solid #ccc; padding: 20px; }}
                    .header, .footer {{ background-color: #f2f2f2; padding: 10px; }}
                    .titulo {{ text-align: center; font-size: 24px; margin-bottom: 20px; }}
                    .datos-emisor, .datos-receptor {{ margin-bottom: 20px; }}
                    .datos-emisor h3, .datos-receptor h3 {{ margin-bottom: 5px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
                    table, th, td {{ border: 1px solid #999; }}
                    th, td {{ padding: 8px; text-align: left; }}
                    .totales {{ margin-top: 20px; text-align: right; }}
                    .totales div {{ margin: 5px 0; }}
                  </style>
                </head>
                <body>
                  <div class='factura'>
                    <div class='header'>
                      <strong>Factura No:</strong> {numero_factura} | <strong>Fecha:</strong> {fecha_factura}
                    </div>

                    <div class='titulo'>Factura Electrónica</div>

                    <div class='datos-emisor'>
                      <h3>Emisor:</h3>
                      <p>{empresa_emisora}<br>
                      RFC: {empresa_emisora_rfc}<br>
                      Dirección: {numero_factura_direccion}</p>
                    </div>

                    <div class='datos-receptor'>
                      <h3>Receptor:</h3>
                      <p>{receptor_nombre}<br>
                      RFC: {receptor_rfc}<br>
                      Uso CFDI: {receptor_cfdi} - {receptor_cfdi_nombre}</p>                      
                      Regimen fiscal: {receptor_regimen} - {receptor_regimen_nombre}</p>

                    </div>

                    <table>
                      <thead>
                        <tr>
                          <th>Cantidad</th>
                          <th>Descripción</th>
                          <th>Precio Unitario</th>
                          <th>Importe</th>
                        </tr>
                      </thead>
                      <tbody>
                        {productos}
                      </tbody>
                    </table>

                    <div class='totales'>
                      <div><strong>Subtotal:</strong> ${venta_subtotal:N2}</div>
                      <div><strong>IVA (16%):</strong> ${venta_iva:N2}</div>
                      <div><strong>Total:</strong> ${venta_total:N2}</div>
                    </div>

                    <div class='footer'>
                      <p>Este documento es una representación impresa de un CFDI.</p>
                    </div>
                  </div>
                </body>
                </html>";


                var mailMessage = new MailMessage(
                    _emailSettings.FromEmail,
                    invoice.Contact,
                    "Factura CFDI | HTML - Refaccionaria Kikis",
                    htmlBody
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

        public bool SendQuote(SaleREQ sale) {
            try {
                // Datos del cliente
                string clienteNombre = $"{sale.Client.FirstName} {sale.Client.LastName}";
                string clienteEmail = sale.Client.Email;
                string clienteTelefono = sale.Client.Cellphone;
                string clienteDireccion = sale.Client.Address;

                // Datos del emisor (estáticos o configurables)
                string empresa_emisora = "KIKIS REFACCIONARIA";
                string empresa_emisora_rfc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string direccion_emisor = "Calle Ficticia 123, COD_POSTAL MERIDA, YUCATAN, MEXICO";

                // Fecha y número de cotización
                string fecha = sale.CreateDate.ToString("yyyy-MM-dd");
                string cotizacionFolio = $"COT-{DateTime.Now:yyyyMMddHHmmss}";

                // Productos
                string productosHtml = "";
                foreach(var product in sale.SaleDetails) {
                    productosHtml += $@"
                    <tr>
                        <td>{product.Quantity}</td>
                        <td>{product.Name}</td>
                        <td>${product.PriceUnit:N2}</td>
                        <td>${product.Total:N2}</td>
                    </tr>";
                }

                // Totales
                string venta_subtotal = sale.SubTotal.ToString("N2");
                string venta_iva = sale.Iva.ToString("N2");
                string venta_total = sale.Total.ToString("N2");

                // HTML del correo
                string htmlBody = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                  <meta charset='UTF-8'>
                  <title>Cotización</title>
                  <style>
                    body {{ font-family: Arial, sans-serif; margin: 40px; color: #333; }}
                    .cotizacion {{ border: 1px solid #ccc; padding: 20px; }}
                    .header, .footer {{ background-color: #f2f2f2; padding: 10px; }}
                    .titulo {{ text-align: center; font-size: 24px; margin-bottom: 20px; }}
                    .datos-emisor, .datos-receptor {{ margin-bottom: 20px; }}
                    .datos-emisor h3, .datos-receptor h3 {{ margin-bottom: 5px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
                    table, th, td {{ border: 1px solid #999; }}
                    th, td {{ padding: 8px; text-align: left; }}
                    .totales {{ margin-top: 20px; text-align: right; }}
                    .totales div {{ margin: 5px 0; }}
                  </style>
                </head>
                <body>
                  <div class='cotizacion'>
                    <div class='header'>
                      <strong>Cotización No:</strong> {cotizacionFolio} | <strong>Fecha:</strong> {fecha}
                    </div>

                    <div class='titulo'>Cotización</div>

                    <div class='datos-emisor'>
                      <h3>Emisor:</h3>
                      <p>{empresa_emisora}<br>
                      RFC: {empresa_emisora_rfc}<br>
                      Dirección: {direccion_emisor}</p>
                    </div>

                    <div class='datos-receptor'>
                      <h3>Cliente:</h3>
                      <p>{clienteNombre}<br>
                      Correo: {clienteEmail}<br>
                      Teléfono: {clienteTelefono}<br>
                      Dirección: {clienteDireccion}</p>
                    </div>

                    <table>
                      <thead>
                        <tr>
                          <th>Cantidad</th>
                          <th>Descripción</th>
                          <th>Precio Unitario</th>
                          <th>Importe</th>
                        </tr>
                      </thead>
                      <tbody>
                        {productosHtml}
                      </tbody>
                    </table>

                    <div class='totales'>
                      <div><strong>Subtotal:</strong> ${venta_subtotal}</div>
                      <div><strong>IVA (16%):</strong> ${venta_iva}</div>
                      <div><strong>Total:</strong> ${venta_total}</div>
                    </div>

                    <div class='footer'>
                      <p>Gracias por su preferencia. Esta es una cotización informativa, no representa una factura.</p>
                    </div>
                  </div>
                </body>
                </html>";

                // Envío de correo
                var mailMessage = new MailMessage(
                    _emailSettings.FromEmail,
                    clienteEmail,
                    "Cotización | Refaccionaria Kikis",
                    htmlBody
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
                throw new Exception("Error al enviar la cotización: " + ex.Message);
            }

            return true;
        }

    }
}
