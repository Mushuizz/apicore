using SqlSugar;

namespace Api.Core.Model.User
{
    public class UserRoleEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string UserRoleId { get; set; }

        public string UserID { get; set; }
        public string RoleId { get; set; }
    }
}
