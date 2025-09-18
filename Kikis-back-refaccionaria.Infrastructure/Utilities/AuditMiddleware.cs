using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Kikis_back_refaccionaria.Infrastructure.Utilities {
    public class AuditMiddleware {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, KikisDbContext dbContext) {

            if(context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                await _next(context);
                return;
            }

            // Captura información de la petición
            string ip = context.Connection.RemoteIpAddress?.ToString();
            string path = context.Request.Path;
            string method = context.Request.Method;
            int user = int.Parse(context.User?.Claims.FirstOrDefault().Value);

            // Leer cuerpo de la petición (opcional)
            context.Request.EnableBuffering();
            string body = "";
            using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true)) {
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Interceptar la respuesta
            var originalBodyStream = context.Response.Body;
            using(var responseBody = new MemoryStream()) {
                context.Response.Body = responseBody;

                int statusCode = 0;
                string responseBodyText = "";

                try {
                    await _next(context);
                    statusCode = context.Response.StatusCode;

                    // Leer el body de la respuesta
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // Copiar de nuevo al stream original
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch(Exception ex) {
                    statusCode = 500;
                    responseBodyText = $"[Error]: {ex.Message}";
                    await responseBody.CopyToAsync(originalBodyStream);
                    throw;
                }

                // Guardar en la base de datos
                var transaction = new TbTransactionHistory {
                    User = user,
                    Path = path,
                    Method = method,
                    IPAddress = ip,
                    Date = DateTime.UtcNow,
                    RequestBody = body,
                    ResponseBody = responseBodyText,  // Aquí guardas la respuesta
                    ResponseStatus = statusCode.ToString()
                };

                dbContext.TbTransactionHistories.Add(transaction);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
