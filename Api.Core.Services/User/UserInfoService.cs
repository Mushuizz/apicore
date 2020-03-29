using Api.Core.Common.Attributes;
using Api.Core.IRepository.User;
using Api.Core.IServices.User;
using Api.Core.Model.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Core.Services.User
{
    public class UserInfoService : BaseService<UserEntity>, IUserInfoService
    {
        public UserInfoService(IUserRepository userRepository)
        {
            base.BaseDal = userRepository;
        }

        [CachingAttribute]
        public async Task<List<UserEntity>> GetAll()
        {
            var result = await base.QueryAsync();
            return result;
        }
        [CachingAttribute]
        public UserEntity test()
        {
            var a = new UserEntity
            {
                UserName = "123",
                Password = "qwe"
            };
            return a;
        }
    }
}
