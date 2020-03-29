using SqlSugar;

namespace Api.Core.Model.User
{
    public class UserEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string UserId { get; set; }
        [SugarColumn(IsNullable = false)]
        public string UserName { get; set; }
        [SugarColumn(IsNullable = false)]
        public string Password { get; set; }


    }
}
