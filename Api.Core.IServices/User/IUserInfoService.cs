using Api.Core.Model.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Core.IServices.User
{
    public interface IUserInfoService : IBaseServices<UserEntity>
    {
        Task<List<UserEntity>> GetAll();
        UserEntity test();


    }
}
