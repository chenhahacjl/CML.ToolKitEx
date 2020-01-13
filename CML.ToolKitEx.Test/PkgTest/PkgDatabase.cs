using System;
using System.Data;
using CML.PkgDatabase;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 泛型结果工具包测试
    /// </summary>
    internal class PkgDatabase : PkgTestBase
    {
        /// <summary>
        /// 测试类名
        /// </summary>
        public override string ClassName => "PkgDatabase";

        /// <summary>
        /// 更新时间
        /// </summary>
        public override string UpdateDate => new VersionInfo().VerDate;

        /// <summary>
        /// 版本信息
        /// </summary>
        public override string VersionInfo => $"{new VersionInfo().VerMain} => {new VersionInfo().VerDev}";

        /// <summary>
        /// 更新信息
        /// </summary>
        public override string UpdateInfo => new VersionInfo().GetVersionInfo();

        /// <summary>
        /// 执行测试
        /// </summary>
        public override bool ExecuteTest()
        {
            bool result = true;

            PrintLog(MsgType.Info, "开始执行Access数据库操作测试");
            if (TestAccess())
            {
                PrintLog(MsgType.PASS, "Access数据库操作测试通过");
            }
            else
            {
                result = false;
                PrintLog(MsgType.FAIL, "Access数据库操作测试未通过");
            }

            PrintLog(MsgType.Info, "开始执行MySQL数据库操作测试");
            if (TestMySql())
            {
                PrintLog(MsgType.PASS, "MySQL数据库操作测试通过");
            }
            else
            {
                result = false;
                PrintLog(MsgType.FAIL, "MySQL数据库操作测试未通过");
            }

            PrintLog(MsgType.Info, "开始执行Oracle数据库操作测试");
            if (TestOracle())
            {
                PrintLog(MsgType.PASS, "Oracle数据库操作测试通过");
            }
            else
            {
                result = false;
                PrintLog(MsgType.FAIL, "Oracle数据库操作测试未通过");
            }

            PrintLog(MsgType.Info, "开始执行SqlServer数据库操作测试");
            if (TestSqlServer())
            {
                PrintLog(MsgType.PASS, "SqlServer数据库操作测试通过！");
            }
            else
            {
                result = false;
                PrintLog(MsgType.FAIL, "SqlServer数据库操作测试未通过");
            }

            return result;
        }

        /// <summary>
        /// Access数据库测试
        /// </summary>
        /// <returns></returns>
        public bool TestAccess()
        {
            return false;
        }

        /// <summary>
        /// MySql数据库测试
        /// </summary>
        /// <returns></returns>
        public bool TestMySql()
        {
            return false;
        }

        /// <summary>
        /// Oracle数据库测试
        /// </summary>
        /// <returns></returns>
        public bool TestOracle()
        {
            string strConnection = "DATA SOURCE=192.168.1.1:9696/CMILE; USER ID=**********; PASSWORD=**********;";
            Database database = new Database(EDatabaseType.ORACLE, strConnection);

            PrintLog(MsgType.Info, "初始化ORACLE数据库！");
            CML.PkgResult.TResult<bool> initResult = database.InitDatabase();
            if (!initResult || !initResult.Result)
            {
                PrintLog(MsgType.FAIL, initResult.ErrorMessage);
                return false;
            }

            PrintLog(MsgType.Info, "打开ORACLE数据库连接！");
            CML.PkgResult.TResult<bool> connResult = database.OpenConnection();
            if (!connResult || !initResult.Result)
            {
                PrintLog(MsgType.FAIL, connResult.ErrorMessage);
                return false;
            }

            PrintLog(MsgType.Info, "查询数据表数量！");
            string strSql =
                "SELECT\n" +
                "    COUNT(TABLE_NAME)\n" +
                "FROM\n" +
                "    ALL_TABLES\n" +
                "WHERE\n" +
                "    OWNER = 'CMILE'\n" +
                "ORDER BY\n" +
                "    TABLE_NAME";
            CML.PkgResult.TResult<object> objResult = database.GetSingleObject(strSql);
            if (!objResult || objResult == null)
            {
                PrintLog(MsgType.FAIL, "查询数据表数量失败");
                return false;
            }
            else if (!int.TryParse(objResult.Result.ToString(), out int count))
            {
                PrintLog(MsgType.FAIL, "数据表数量数据类型转换失败！");
                return false;
            }
            else
            {
                PrintLog(MsgType.Info, "数据表数量: " + count);
            }

            PrintLog(MsgType.Info, "查询数据表名称！");
            strSql =
                 "SELECT\n" +
                 "    TABLE_NAME\n" +
                 "FROM\n" +
                 "    ALL_TABLES\n" +
                 "WHERE\n" +
                 "    OWNER = 'SCOTT'\n" +
                 "ORDER BY\n" +
                 "    TABLE_NAME";
            CML.PkgResult.TResult<DataTable> dtResult = database.ExecuteQuery(strSql);
            if (!dtResult)
            {
                PrintLog(MsgType.FAIL, dtResult.ErrorMessage);
                return false;
            }
            else
            {
                string tableName = "";
                for (int i = 0; i < dtResult.Result.Rows.Count; i++)
                {
                    tableName += dtResult.Result.Rows[i][0].ToString() + ", ";
                }
                PrintLog(MsgType.Info, "数据表名称: " + tableName.Trim().TrimEnd(','));
            }

            return true;
        }

        /// <summary>
        /// SqlServer数据库测试
        /// </summary>
        /// <returns></returns>
        public bool TestSqlServer()
        {
            return false;
        }
    }
}
