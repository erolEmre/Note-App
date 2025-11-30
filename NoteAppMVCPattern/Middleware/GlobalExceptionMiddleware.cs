using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoteApp.WebUI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var isDev = _env.IsDevelopment();

                Console.Error.WriteLine(ex);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Beklenmeyen bir hata oluştu.",
                    detail = isDev ? ex.ToString() : null
                };

                var json = JsonSerializer.Serialize(response);
                if (httpContext.Request.Path.StartsWithSegments("/api"))
                {
                    await httpContext.Response.WriteAsync(json);
                }
                else
                {
                    // Razor hata sayfasına yönlendir
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                        statusCode = httpContext.Response.StatusCode,
                        Title = "Hata!"
                    }));
                }
               
            }
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
