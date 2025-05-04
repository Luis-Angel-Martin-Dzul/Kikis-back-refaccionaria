using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kikis_back_refaccionaria.Infrastructure.Filters {
    public class ValidationFilter : IAsyncActionFilter {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            if(!context.ModelState.IsValid) {

                var messages = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response = new {
                    status = 400,
                    title = "Validation Error",
                    detail = messages,
                    data = ""
                };

                context.Result = new JsonResult(response);
                return;
            }
            await next();
        }
    }
}
