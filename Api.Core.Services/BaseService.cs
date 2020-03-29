using Api.Core.IRepository;
using Api.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Services
{
    public class BaseService<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        public IBaseRepository<TEntity> BaseDal;


        public async Task<List<TEntity>> Query()
        {
            return await BaseDal.Query();
        }

        //[CachingAttribute]
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await BaseDal.Query(whereExpression);
        }

        public Task<List<TEntity>> QueryAsync()
        {
            return BaseDal.QueryAsync();
        }


    }
}
