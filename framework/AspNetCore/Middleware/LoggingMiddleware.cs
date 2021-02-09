using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Middleware
{
    public class LoggingMiddleware
    {
        private static readonly Action<ILogger, string, string, int?, double, System.Exception> RoutePerformance = LoggerMessage.Define<string, string, int?, double>(LogLevel.Information, 0, "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");

        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IWebHostEnvironment hostEnvironment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        public async Task Invoke(HttpContext context, IEnvironmentInfoService environmentInfoService)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
                sw.Stop();

                var statusCode = context.Response?.StatusCode;

                if (statusCode >= 500)
                {
                    using (_logger.BeginScope(environmentInfoService.Dump()))
                    using (_logger.BeginScope(BuildRequestInfo(context.Request)))
                    {
                        RoutePerformance(_logger, context.Request.Method, context.Request.Path.ToString(),
                            statusCode,
                            sw.Elapsed.TotalMilliseconds, null);
                    }
                }
                else
                {
                    RoutePerformance(_logger, context.Request.Method, context.Request.Path.ToString(),
                        statusCode,
                        sw.Elapsed.TotalMilliseconds, null);
                }
            }
            catch (System.Exception ex)
            {
                using (_logger.BeginScope(environmentInfoService.Dump()))
                using (_logger.BeginScope(BuildRequestInfo(context.Request)))
                {
                    LogException(context, sw.Elapsed, ex);
                    if (!_hostEnvironment.IsDevelopment() && context.Response != null)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = 500;
                    }
                    else
                    {
                        throw;
                    }

                }
            }
        }

        private void LogException(HttpContext httpContext, TimeSpan elapsed, System.Exception ex)
        {
            RoutePerformance(_logger, httpContext.Request.Method, httpContext.Request.Path.ToString(), 500, elapsed.TotalMilliseconds, ex);
        }

        private static IDictionary<string, string> BuildRequestInfo(HttpRequest request)
        {
            var result = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            result.Add("RequestHost", request.Host.ToString());
            result.Add("RequestProtocol", request.Protocol);

            return result;
        }
    }
}