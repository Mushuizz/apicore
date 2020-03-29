using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Api.Core.Filter
{
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        private ILogger<GlobalExceptionsFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionsFilter(ILogger<GlobalExceptionsFilter> logger,
             IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            string message;
            if (_env.IsDevelopment())
            {
                message = context.Exception.StackTrace;
            }
            else
            {
                message = context.Exception.Message;
            }

            context.Result = new InternalServerErrorObjectResult(message);

            _logger.LogError(message + WriteLog(message, context.Exception));
        }

        public string WriteLog(string throwMsg, Exception ex)
        {
            return string.Format("\r\n【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}", new object[] { throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace });
        }
    }

    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
