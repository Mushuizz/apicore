using System.ComponentModel.DataAnnotations;

namespace Api.Core.Model.User
{
    public class UserCredentials
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RefreshCredentials
    {
        [Required]
        public string OldToken { get; set; }
    }
}
