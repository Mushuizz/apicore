using Api.Core.Auth;
using Api.Core.Common.Auth;
using Api.Core.Common.Cache;
using Api.Core.Common.Helper;
using Api.Core.IServices.User;
using Api.Core.Model.User;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LoginController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthServer _authServer;
        private readonly IUserInfoService _userInfo;
        private readonly IRedisCache _cache;
        private readonly AuthSetting _authSetting;

        public LoginController(IMapper mapper,
            IAuthServer authServer
            , IUserInfoService userInfo,
            IRedisCache cache,
            IOptions<AuthSetting> authSetting)
        {
            _mapper = mapper;
            _authServer = authServer;
            _userInfo = userInfo;
            _cache = cache;
            _authSetting = authSetting.Value;
        }


        [HttpGet("get")]
        //[Authorize]
        //[Authorize(Roles ="超级管理员")]
        //[Authorize(Roles = "123")]
        //[Authorize(Roles = "管理员")]
        public async Task<List<UserEntity>> Get()
        {
            var a = new UserCredentials
            {
                UserName = "123",
                Password = "ewe"
            };
            var ass = await _userInfo.GetAll();
            // _cache.Set("zsm", "test1", TimeSpan.FromSeconds(600));
            return ass;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userCredentials"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserCredentials userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userCredentials.Password = MD5Helper.MD5Encrypt32(userCredentials.Password);

            var userInfo = await _userInfo.Query(x => x.UserName == userCredentials.UserName && x.Password == userCredentials.Password);

            if (userInfo.Count > 0)
            {
                var userModel = userInfo.FirstOrDefault();

                var cacheToken = _cache.GetValue(userModel.UserId);
                if (!string.IsNullOrWhiteSpace(cacheToken))
                {
                    var Result = new AuthResult()
                    {
                        Success = true,
                        Token = cacheToken.Replace("\"", ""),
                        Message = "请勿重复请求Token，返回当前有效Token！"
                    };
                    return Ok(Result);
                }

                var response = await _authServer.CreateAuthentication(userModel);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                _cache.Set(userModel.UserId, response.Token, _authSetting.TokenLifetime * 2);
                return Ok(response);
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "认证失败"
                });
            }
        }

        [HttpGet]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshCredentials refreshCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var JwtModel = _authServer.SerializeJwt(refreshCredentials);
            if (JwtModel == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "token无效，请重新登录！"
                });
            }
            var userInfo = await _userInfo.Query(x => x.UserId == JwtModel.UserId);

            if (userInfo != null)
            {
                var userModel = userInfo.FirstOrDefault();

                var cacheToken = _cache.GetValue(userModel.UserId);
                if (string.IsNullOrWhiteSpace(cacheToken))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "token已过期，请重新登录！"
                    });
                }

                var response = await _authServer.CreateAuthentication(userInfo.FirstOrDefault());
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                _cache.Set(userModel.UserId, response.Token, _authSetting.TokenLifetime * 2);

                return Ok(response);
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "认证失败"
                });
            }
        }
    }
}