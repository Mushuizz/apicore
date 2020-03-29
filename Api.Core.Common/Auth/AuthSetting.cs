using System;

namespace Api.Core.Common.Auth
{
    public class AuthSetting
    {

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}
