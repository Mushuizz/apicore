using Api.Core.Common.Attributes;
using Api.Core.Common.Cache;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.AOP
{
    /// <summary>
    /// 只能缓存不带参数的方法!!!!!!!!
    /// </summary>
    public class CacheInterceptor : IInterceptor
    {
        private readonly IRedisCache _cache;

        public CacheInterceptor(IRedisCache cache)
        {
            _cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.ReturnType == typeof(void) || method.ReturnType == typeof(Task))
            {
                invocation.Proceed();
                return;
            }
            //对当前方法的特性验证
            var CachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;

            if (CachingAttribute != null)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                //注意是 string 类型，方法GetValue
                var cacheValue = _cache.GetValue(cacheKey);
                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    Type returnType;
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();
                    }
                    else
                    {
                        returnType = method.ReturnType;
                    }

                    dynamic _result = JsonConvert.DeserializeObject(cacheValue, returnType);
                    invocation.ReturnValue = (typeof(Task).IsAssignableFrom(method.ReturnType)) ? Task.FromResult(_result) : _result;
                    return;
                }
                //去执行当前的方法
                invocation.Proceed();

                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;

                    //Type type = invocation.ReturnValue?.GetType();
                    var type = invocation.Method.ReturnType;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }
                    if (response == null) response = string.Empty;

                    _cache.Set(cacheKey, response, TimeSpan.FromMinutes(CachingAttribute.ExpirationMinutes));
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }


        public string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            string key = $"{typeName}:{methodName}";
            return key;
        }
    }
}
