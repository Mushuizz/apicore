namespace Api.Core.Model.User
{
    public class AuthResult
    {
        public string Token { get; set; }
        public double ExpirySecond { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
