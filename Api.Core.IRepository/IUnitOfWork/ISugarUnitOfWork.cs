using SqlSugar;

namespace Api.Core.IRepository.IUnitOfWork
{
    public interface ISugarUnitOfWork
    {
        SqlSugarClient GetDbClient();

        void BeginTran();

        void CommitTran();
        void RollbackTran();
    }
}
