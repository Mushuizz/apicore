using SqlSugar;

namespace Api.Core.Model.User
{
    public class RoleEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
