using Api.Core.IRepository.User;
using Api.Core.IServices.User;
using Api.Core.Model.User;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Services.User
{
    public class UserRoleInfoService : BaseService<UserRoleEntity>, IUserRoleInfoService
    {
        private readonly IRoleRepository _roleRepository;

        public UserRoleInfoService(IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository)
        {
            _roleRepository = roleRepository;
            base.BaseDal = userRoleRepository;
        }
        public async Task<string> userRoleEntities(string userId)
        {
            string roleName = default;
            var roleList = await _roleRepository.Query();
            var userRoleList = await base.Query(x => x.UserID == userId);
            var arr = userRoleList.Select(x => x.RoleId).ToList();
            var roles = roleList.Where(d => arr.Contains(d.RoleId));
            roleName = string.Join(',', roles.Select(r => r.RoleName).ToArray());
            return roleName;
        }
    }
}
