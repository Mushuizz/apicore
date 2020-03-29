using Api.Core.Common.Helper;
using StackExchange.Redis;
using System;

namespace Api.Core.Common.Cache
{
    public class RedisCache : IRedisCache
    {
        private readonly string redisConnenctionString;

        public volatile ConnectionMultiplexer redisConnection;

        private readonly object redisConnectionLock = new object();

        private static string redisConfiguration = Appsetting.app(new string[] { "AppSettings", "RedisConnectionString" });

        public RedisCache()
        {
            if (!string.IsNullOrWhiteSpace(redisConfiguration))
            {
                redisConnenctionString = redisConfiguration;
                redisConnection = GetRedisConnection();
            }
            else
            {
                throw new Exception("redisConnenctionString is empty");
            }
        }
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (redisConnection != null && redisConnection.IsConnected)
            {
                return redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (redisConnection != null)
                {
                    //释放redis连接
                    redisConnection.Dispose();
                }
                try
                {
                    redisConnection = ConnectionMultiplexer.Connect(redisConnenctionString);
                }
                catch (Exception)
                {
                    throw new Exception("Redis服务连接失败!");
                }
            }
            return redisConnection;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Get(string key)
        {
            return redisConnection.GetDatabase().KeyExists(key);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return redisConnection.GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(string key)
        {
            var value = redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            redisConnection.GetDatabase().KeyDelete(key);
        }



        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        public void Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                redisConnection.GetDatabase().StringSet(key, SerializeHelper.Serialize(value), cacheTime);
            }
        }

        public void Set(string key, object value)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                redisConnection.GetDatabase().StringSet(key, SerializeHelper.Serialize(value));
            }
        }




        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, byte[] value)
        {
            return redisConnection.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(120));
        }
    }
}
