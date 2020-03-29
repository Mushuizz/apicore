using System;

namespace Api.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingAttribute : Attribute
    {
        public int ExpirationMinutes { get; set; } = 30;
    }
}
