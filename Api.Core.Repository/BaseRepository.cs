using Api.Core.IRepository;

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private ISqlSugarClient _db;

        public BaseRepository(ISqlSugarClient context)
        {
            _db = context;
        }

        public async Task<List<TEntity>> Query()
        {
            return await _db.Queryable<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        public async Task<List<TEntity>> QueryAsync()
        {
            return await _db.Queryable<TEntity>().ToListAsync();
        }


    }
}
