using Api.Core.Model.User;
using System.Threading.Tasks;

namespace Api.Core.Auth
{
    public interface IAuthServer
    {
        Task<AuthResult> CreateAuthentication(UserEntity user);
        TokenModelJwt SerializeJwt(RefreshCredentials refreshCredentials);
    }
}
