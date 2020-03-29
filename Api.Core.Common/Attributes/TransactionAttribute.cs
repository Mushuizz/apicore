using System;

namespace Api.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TransactionAttribute : Attribute
    {
    }
}
