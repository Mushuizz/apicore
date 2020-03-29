using System;

namespace Api.Core.Common.Cache
{
    public interface IMemoryCaching
    {
        object MemoryCachGet(string cacheKey);
        void MemoryCachSet(string cacheKey, object cacheValue);
        void MemoryCachSet(string cacheKey, object cacheValue, TimeSpan timeSpan);
    }
}
