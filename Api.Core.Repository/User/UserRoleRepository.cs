using Api.Core.IRepository.User;
using Api.Core.Model.User;
using SqlSugar;

namespace Api.Core.Repository.User
{
    public class UserRoleRepository : BaseRepository<UserRoleEntity>, IUserRoleRepository
    {
        public UserRoleRepository(ISqlSugarClient context) : base(context)
        {

        }
    }
}
