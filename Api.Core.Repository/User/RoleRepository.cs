using Api.Core.IRepository.User;
using Api.Core.Model.User;
using SqlSugar;

namespace Api.Core.Repository.User
{
    public class RoleRepository : BaseRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(ISqlSugarClient context) : base(context)
        {

        }
    }
}
