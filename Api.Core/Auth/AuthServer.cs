using Api.Core.Common.Auth;
using Api.Core.IServices.User;
using Api.Core.Model.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Core.Auth
{
    public class AuthServer : IAuthServer
    {
        private readonly IUserRoleInfoService _userRoleInfoService;
        private readonly IUserInfoService _userInfo;
        private readonly AuthSetting _authSetting;

        public AuthServer(IOptions<AuthSetting> authSetting
            , IUserInfoService userInfo,
            IUserRoleInfoService userRoleInfoService)
        {
            _userRoleInfoService = userRoleInfoService;
            _userInfo = userInfo;
            _authSetting = authSetting.Value;
        }
        public async Task<AuthResult> CreateAuthentication(UserEntity user)
        {
            var userRoles = await _userRoleInfoService.userRoleEntities(user.UserId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSetting.Secret);
            var now = DateTime.Now;
            var claims = new List<Claim> {
             new Claim(ClaimTypes.Name, user.UserName),
             new Claim(ClaimTypes.NameIdentifier, user.UserId),
             new Claim(ClaimTypes.Expiration, now.AddSeconds(_authSetting.TokenLifetime.TotalSeconds).ToString())};
            //foreach (var item in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, item.role));
            //}
            claims.AddRange(userRoles.Split(',').Select(x => new Claim(ClaimTypes.Role, x)));

            var token = new JwtSecurityToken
            (
                claims: claims,
                issuer: _authSetting.Issuer,
                audience: _authSetting.Audience,
                notBefore: now,
                expires: now.Add(_authSetting.TokenLifetime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            var strToken = tokenHandler.WriteToken(token);

            return new AuthResult
            {
                Success = true,
                Token = strToken,
                ExpirySecond = _authSetting.TokenLifetime.TotalSeconds //过期时间
            };
        }

        public TokenModelJwt SerializeJwt(RefreshCredentials refreshCredentials)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(refreshCredentials.OldToken);

                bool isHave = jwtToken.Payload.ContainsKey(ClaimTypes.NameIdentifier);
                object uid = default;
                if (isHave)
                {
                    jwtToken.Payload.TryGetValue(ClaimTypes.NameIdentifier, out uid);
                }
                return new TokenModelJwt()
                {
                    UserId = uid.ToString()
                };
            }
            catch (Exception)
            {
                return default;
            }

        }
    }
}
