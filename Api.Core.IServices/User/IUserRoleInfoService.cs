using Api.Core.Model.User;
using System.Threading.Tasks;

namespace Api.Core.IServices.User
{
    public interface IUserRoleInfoService : IBaseServices<UserRoleEntity>
    {
        Task<string> userRoleEntities(string userId);
    }
}
