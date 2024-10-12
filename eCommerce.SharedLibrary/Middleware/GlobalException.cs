using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware;

    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Something went wrong. Please try again.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // check if response is too many request // 429
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = " Too many requests. Please try again.";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // check if response is unauth // 401
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Unauthorized";
                    message = "You are not authorized to access this resource.";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // check if response is forbidden // 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed to access.";
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log original exception
               LogException.LogExceptions(ex);

               if (ex is TaskCanceledException || ex is OperationCanceledException || ex is TimeoutException)
               {
                   title = "Timeout";
                   message = "The operation has timed out.";
                   statusCode = StatusCodes.Status408RequestTimeout;
               }
               
               // default
               await ModifyHeader(context, title, message, statusCode);
            }
            
            
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);
            
        }
    }