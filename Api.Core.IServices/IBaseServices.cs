﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.IServices
{
    public interface IBaseServices<TEntity> where TEntity : class
    {
        Task<List<TEntity>> Query();
        Task<List<TEntity>> QueryAsync();

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
    }
}
