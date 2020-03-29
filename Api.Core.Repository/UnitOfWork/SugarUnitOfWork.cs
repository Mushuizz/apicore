using Api.Core.IRepository.IUnitOfWork;
using SqlSugar;
using System;

namespace Api.Core.Repository.UnitOfWork
{
    public class SugarUnitOfWork : ISugarUnitOfWork
    {
        private ISqlSugarClient _sqlSugarClient;

        public SugarUnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public SqlSugarClient GetDbClient()
        {
            // 必须要as，后边会用到切换数据库操作
            return _sqlSugarClient as SqlSugarClient;
        }

        public void BeginTran()
        {
            GetDbClient().BeginTran();
        }

        public void CommitTran()
        {
            try
            {
                GetDbClient().CommitTran();
            }
            catch (Exception ex)
            {
                GetDbClient().RollbackTran();
                throw ex;
            }
        }

        public void RollbackTran()
        {
            GetDbClient().RollbackTran();
        }
    }
}
