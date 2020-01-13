using System.Collections.Generic;

namespace CML.PkgDatabase
{
    /// <summary>
    /// 事务执行参数模型
    /// </summary>
    public class MTransactionParameter
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SqlString { get; }

        /// <summary>
        /// SQL语句参数数组
        /// </summary>
        public MDataParameter[] Parameters { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        public MTransactionParameter(string strSql)
        {
            SqlString = strSql;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        public MTransactionParameter(string strSql, MDataParameter[] parameters)
        {
            SqlString = strSql;
            Parameters = parameters;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        public MTransactionParameter(string strSql, List<MDataParameter> parameters)
        {
            SqlString = strSql;
            Parameters = parameters.ToArray();
        }
    }
}
