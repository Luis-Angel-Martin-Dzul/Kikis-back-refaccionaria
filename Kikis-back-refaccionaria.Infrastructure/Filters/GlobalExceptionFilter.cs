using Kikis_back_refaccionaria.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Kikis_back_refaccionaria.Infrastructure.Filters {
    public class GlobalExceptionFilter : IExceptionFilter {

        public void OnException(ExceptionContext context) {
            if(context.Exception.GetType() == typeof(BusinessException)) {
                var exception = (BusinessException)context.Exception;
                var validation = new {
                    Status = 400,
                    Title = "Bad Request",
                    Detail = exception.Message,
                    Data = default(object)
                };

                context.Result = new BadRequestObjectResult(validation);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
            }
        }
    }
}
