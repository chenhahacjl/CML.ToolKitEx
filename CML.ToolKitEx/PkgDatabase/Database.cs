using CML.PkgDatabase.DatabaseBase;
using CML.PkgResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace CML.PkgDatabase
{
    /// <summary>
    /// 数据库帮助类
    /// </summary>
    public class Database
    {
        #region 私有变量
        /// <summary>
        /// 数据库底层
        /// </summary>
        private IDatabaseBase m_iDatabaseBase = null;
        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDbConnection m_iConn = null;
        /// <summary>
        /// 数据库执行命令
        /// </summary>
        private IDbCommand m_iCmd = null;
        /// <summary>
        /// 数据库初始化标志
        /// </summary>
        private bool m_isInitDatabase = false;
        #endregion

        #region 公共属性
        /// <summary>
        /// 数据库类型
        /// </summary>
        public EDatabaseType ConnectionType { get; set; } = EDatabaseType.NONE;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// 是否自动关闭数据库连接(默认开启自动关闭)
        /// </summary>
        public bool IsAutoCloseConn { get; set; } = true;
        #endregion

        #region 构造函数
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        public Database() { }

        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <param name="dbType">连接数据库类型</param>
        /// <param name="strConnStr">数据库连接字符串</param>
        public Database(EDatabaseType dbType, string strConnStr)
        {
            ConnectionType = dbType;
            ConnectionString = strConnStr;
        }
        #endregion

        #region 连接操作方法
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public TResult<bool> InitDatabase()
        {
            switch (ConnectionType)
            {
                case EDatabaseType.ACCESS:
                {
                    m_iDatabaseBase = new DatabaseAccess();
                    break;
                }
                case EDatabaseType.MYSQL:
                {
                    m_iDatabaseBase = new DatabaseMySql();
                    break;
                }
                case EDatabaseType.ORACLE:
                {
                    m_iDatabaseBase = new DatabaseOracle();
                    break;
                }
                case EDatabaseType.SQLSERVER:
                {
                    m_iDatabaseBase = new DatabaseSqlServer();
                    break;
                }
                default:
                {
                    m_isInitDatabase = false;
                    return new TResult<bool>(false, $"数据库类型错误或未设置！");
                }
            }

            try
            {
                m_iDatabaseBase.ConnectionString = ConnectionString;
                m_iConn = m_iDatabaseBase.CreateConnection();
                m_iCmd = m_iDatabaseBase.CreateCommand();
                m_iCmd.Connection = m_iConn;

                m_isInitDatabase = true;
            }
            catch (FileNotFoundException)
            {
                m_iDatabaseBase = null;
                m_iConn = null;
                m_iCmd = null;

                m_isInitDatabase = false;

                return new TResult<bool>(false, $"缺少运行依赖程序: {m_iDatabaseBase.RuntimeDepend}");
            }
            catch (Exception ex)
            {
                m_iDatabaseBase = null;
                m_iConn = null;
                m_iCmd = null;

                m_isInitDatabase = false;

                return new TResult<bool>(false, ex);
            }

            return new TResult<bool>(true);
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public TResult<bool> OpenConnection()
        {
            if (!m_isInitDatabase)
            {
                return new TResult<bool>(false, "数据库未初始化！");
            }

            try
            {
                m_iConn.Open();
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex);
            }

            return new TResult<bool>(true);
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public TResult<bool> CloseConnection()
        {
            if (m_isInitDatabase)
            {
                try
                {
                    m_iConn.Close();
                }
                catch (Exception ex)
                {
                    return new TResult<bool>(false, ex);
                }
            }

            return new TResult<bool>(true);
        }
        #endregion

        #region SQL执行方法
        /// <summary>
        /// 执行SQL语句，返回数据表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>数据表</returns>
        public TResult<DataTable> ExecuteQuery(string strSql)
        {
            DataTable dtResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                IDbDataAdapter iAdapter = m_iDatabaseBase.CreateDataAdapter();
                iAdapter.SelectCommand = m_iCmd;

                DataSet dsResult = new DataSet();
                iAdapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables.Count != 0)
                {
                    dtResult = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return new TResult<DataTable>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<DataTable>(dtResult);
        }

        /// <summary>
        /// 执行SQL语句，返回数据表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameter">参数</param>
        /// <returns>数据表</returns>
        public TResult<DataTable> ExecuteQuery(string strSql, MDataParameter dataParameter)
        {
            DataTable dtResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameter != null)
                {
                    IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                    iParameter.ParameterName = dataParameter.Name;
                    iParameter.Value = dataParameter.Value;
                    iParameter.DbType = dataParameter.DataType;

                    m_iCmd.Parameters.Add(iParameter);
                }

                IDbDataAdapter iAdapter = m_iDatabaseBase.CreateDataAdapter();
                iAdapter.SelectCommand = m_iCmd;

                DataSet dsResult = new DataSet();
                iAdapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables.Count != 0)
                {
                    dtResult = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return new TResult<DataTable>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<DataTable>(dtResult);
        }

        /// <summary>
        /// 执行SQL语句，返回数据表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数数组</param>
        /// <returns>数据表</returns>
        public TResult<DataTable> ExecuteQuery(string strSql, MDataParameter[] dataParameters)
        {
            DataTable dtResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Length != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                IDbDataAdapter iAdapter = m_iDatabaseBase.CreateDataAdapter();
                iAdapter.SelectCommand = m_iCmd;

                DataSet dsResult = new DataSet();
                iAdapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables.Count != 0)
                {
                    dtResult = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return new TResult<DataTable>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<DataTable>(dtResult);
        }

        /// <summary>
        /// 执行SQL语句，返回数据表
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数列表</param>
        /// <returns>数据表</returns>
        public TResult<DataTable> ExecuteQuery(string strSql, List<MDataParameter> dataParameters)
        {
            DataTable dtResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Count != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                IDbDataAdapter iAdapter = m_iDatabaseBase.CreateDataAdapter();
                iAdapter.SelectCommand = m_iCmd;

                DataSet dsResult = new DataSet();
                iAdapter.Fill(dsResult);

                if (dsResult != null && dsResult.Tables.Count != 0)
                {
                    dtResult = dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return new TResult<DataTable>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<DataTable>(dtResult);
        }

        /// <summary>
        /// 执行SQL语句，返回影响记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>影响记录数</returns>
        public TResult<int> ExecuteNonQuery(string strSql)
        {
            int nResult = -1;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                nResult = m_iCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new TResult<int>(-1, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<int>(nResult);
        }

        /// <summary>
        /// 执行SQL语句，返回影响记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameter">参数</param>
        /// <returns>影响记录数</returns>
        public TResult<int> ExecuteNonQuery(string strSql, MDataParameter dataParameter)
        {
            int nResult = -1;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameter != null)
                {
                    IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                    iParameter.ParameterName = dataParameter.Name;
                    iParameter.Value = dataParameter.Value;
                    iParameter.DbType = dataParameter.DataType;

                    m_iCmd.Parameters.Add(iParameter);
                }

                nResult = m_iCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new TResult<int>(-1, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<int>(nResult);
        }

        /// <summary>
        /// 执行SQL语句，返回影响记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数数组</param>
        /// <returns>影响记录数</returns>
        public TResult<int> ExecuteNonQuery(string strSql, MDataParameter[] dataParameters)
        {
            int nResult = -1;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Length != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                nResult = m_iCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new TResult<int>(-1, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<int>(nResult);
        }

        /// <summary>
        /// 执行SQL语句，返回影响记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数列表</param>
        /// <returns>影响记录数</returns>
        public TResult<int> ExecuteNonQuery(string strSql, List<MDataParameter> dataParameters)
        {
            int nResult = -1;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Count != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                nResult = m_iCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new TResult<int>(-1, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<int>(nResult);
        }

        /// <summary>
        /// 执行SQL语句，返回结果集中的第一行的第一列
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>结果集中的第一行的第一列</returns>
        public TResult<object> GetSingleObject(string strSql)
        {
            object objResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                objResult = m_iCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return new TResult<object>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<object>(objResult);
        }

        /// <summary>
        /// 执行SQL语句，返回结果集中的第一行的第一列
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameter">参数</param>
        /// <returns>结果集中的第一行的第一列</returns>
        public TResult<object> GetSingleObject(string strSql, MDataParameter dataParameter)
        {
            object objResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameter != null)
                {
                    IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                    iParameter.ParameterName = dataParameter.Name;
                    iParameter.Value = dataParameter.Value;
                    iParameter.DbType = dataParameter.DataType;

                    m_iCmd.Parameters.Add(iParameter);
                }

                objResult = m_iCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return new TResult<object>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<object>(objResult);
        }

        /// <summary>
        /// 执行SQL语句，返回结果集中的第一行的第一列
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数数组</param>
        /// <returns>结果集中的第一行的第一列</returns>
        public TResult<object> GetSingleObject(string strSql, MDataParameter[] dataParameters = null)
        {
            object objResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Length != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                objResult = m_iCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return new TResult<object>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<object>(objResult);
        }

        /// <summary>
        /// 执行SQL语句，返回结果集中的第一行的第一列
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="dataParameters">参数列表</param>
        /// <returns>结果集中的第一行的第一列</returns>
        public TResult<object> GetSingleObject(string strSql, List<MDataParameter> dataParameters = null)
        {
            object objResult = null;

            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }

                m_iCmd.Parameters.Clear();
                m_iCmd.CommandText = strSql;
                m_iCmd.CommandType = CommandType.Text;

                if (dataParameters != null && dataParameters.Count != 0)
                {
                    foreach (MDataParameter dataParameter in dataParameters)
                    {
                        IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                        iParameter.ParameterName = dataParameter.Name;
                        iParameter.Value = dataParameter.Value;
                        iParameter.DbType = dataParameter.DataType;

                        m_iCmd.Parameters.Add(iParameter);
                    }
                }

                objResult = m_iCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return new TResult<object>(null, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<object>(objResult);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="strSqls">事务执行SQL数组</param>
        /// <returns>执行结果</returns>
        public TResult<bool> ExecuteTransaction(string[] strSqls)
        {
            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex);
            }

            IDbTransaction iTransaction = m_iConn.BeginTransaction();
            try
            {
                foreach (string strSql in strSqls)
                {
                    m_iCmd.Parameters.Clear();
                    m_iCmd.CommandText = strSql;
                    m_iCmd.Transaction = iTransaction;
                    m_iCmd.CommandType = CommandType.Text;

                    m_iCmd.ExecuteNonQuery();
                }

                iTransaction.Commit();
            }
            catch (Exception ex)
            {
                iTransaction.Rollback();
                return new TResult<bool>(false, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<bool>(true);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="strSqls">事务执行SQL列表</param>
        /// <returns>执行结果</returns>
        public TResult<bool> ExecuteTransaction(List<string> strSqls)
        {
            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex);
            }

            IDbTransaction iTransaction = m_iConn.BeginTransaction();
            try
            {
                foreach (string strSql in strSqls)
                {
                    m_iCmd.Parameters.Clear();
                    m_iCmd.CommandText = strSql;
                    m_iCmd.Transaction = iTransaction;
                    m_iCmd.CommandType = CommandType.Text;

                    m_iCmd.ExecuteNonQuery();
                }

                iTransaction.Commit();
            }
            catch (Exception ex)
            {
                iTransaction.Rollback();
                return new TResult<bool>(false, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<bool>(true);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="parameters">事务执行参数数组</param>
        /// <returns>执行结果</returns>
        public TResult<bool> ExecuteTransaction(MTransactionParameter[] parameters)
        {
            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex);
            }

            IDbTransaction iTransaction = m_iConn.BeginTransaction();
            try
            {
                foreach (MTransactionParameter item in parameters)
                {
                    m_iCmd.Parameters.Clear();
                    m_iCmd.CommandText = item.SqlString;
                    m_iCmd.Transaction = iTransaction;
                    m_iCmd.CommandType = CommandType.Text;

                    if (item.Parameters != null && item.Parameters.Length != 0)
                    {
                        foreach (MDataParameter parameter in item.Parameters)
                        {
                            IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                            iParameter.ParameterName = parameter.Name;
                            iParameter.Value = parameter.Value;
                            iParameter.DbType = parameter.DataType;

                            m_iCmd.Parameters.Add(iParameter);
                        }
                    }

                    m_iCmd.ExecuteNonQuery();
                }

                iTransaction.Commit();
            }
            catch (Exception ex)
            {
                iTransaction.Rollback();
                return new TResult<bool>(false, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<bool>(true);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="parameters">事务执行参数列表</param>
        /// <returns>执行结果</returns>
        public TResult<bool> ExecuteTransaction(List<MTransactionParameter> parameters)
        {
            try
            {
                if (m_iConn.State == ConnectionState.Broken || m_iConn.State == ConnectionState.Closed)
                {
                    m_iConn.Open();
                }
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex);
            }

            IDbTransaction iTransaction = m_iConn.BeginTransaction();
            try
            {
                foreach (MTransactionParameter item in parameters)
                {
                    m_iCmd.Parameters.Clear();
                    m_iCmd.CommandText = item.SqlString;
                    m_iCmd.Transaction = iTransaction;
                    m_iCmd.CommandType = CommandType.Text;

                    if (item.Parameters != null && item.Parameters.Length != 0)
                    {
                        foreach (MDataParameter parameter in item.Parameters)
                        {
                            IDataParameter iParameter = m_iDatabaseBase.CreateDataParameter(m_iCmd);
                            iParameter.ParameterName = parameter.Name;
                            iParameter.Value = parameter.Value;
                            iParameter.DbType = parameter.DataType;

                            m_iCmd.Parameters.Add(iParameter);
                        }
                    }

                    m_iCmd.ExecuteNonQuery();
                }

                iTransaction.Commit();
            }
            catch (Exception ex)
            {
                iTransaction.Rollback();
                return new TResult<bool>(false, ex);
            }
            finally
            {
                if (IsAutoCloseConn)
                {
                    m_iConn.Close();
                }
            }

            return new TResult<bool>(true);
        }
        #endregion
    }
}
