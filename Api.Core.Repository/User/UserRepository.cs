using Api.Core.IRepository.User;
using Api.Core.Model.User;
using SqlSugar;

namespace Api.Core.Repository.User
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(ISqlSugarClient context) : base(context)
        {

        }
    }
}
