using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenToOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Extract request header
        var signedHeader = httpContext.Request.Headers["Api-Gateway"];
        // nNULL means the request is not coming from the Api Gateway
        if (signedHeader.FirstOrDefault() is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await httpContext.Response.WriteAsync("service unavailable");
            return;

        }else
        {
            await next(httpContext);
        }
    }
}