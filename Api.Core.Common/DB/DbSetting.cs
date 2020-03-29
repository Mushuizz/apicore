using Api.Core.Common.Helper;

namespace Api.Core.Common.DB
{
    public static class DbSetting
    {
        public static int Dbtype = int.Parse(Appsetting.app(new string[] { "UseDb", "DbType" }));

        public static string connectionStrings = Appsetting.app(new string[] { "ConnectionStrings", "DB" });
    }
}
