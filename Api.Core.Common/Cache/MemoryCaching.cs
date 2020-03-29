using Microsoft.Extensions.Caching.Memory;
using System;

namespace Api.Core.Common.Cache
{
    public class MemoryCaching : IMemoryCaching
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCaching(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public object MemoryCachGet(string cacheKey)
        {
            return _memoryCache.Get(cacheKey);
        }
        public void MemoryCachSet(string cacheKey, object cacheValue, TimeSpan timeSpan)
        {
            _memoryCache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }

        public void MemoryCachSet(string cacheKey, object cacheValue)
        {
            _memoryCache.Set(cacheKey, cacheValue);
        }
    }
}
