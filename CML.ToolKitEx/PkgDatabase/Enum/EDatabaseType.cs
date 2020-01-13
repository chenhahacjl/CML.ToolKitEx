﻿namespace CML.PkgDatabase
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum EDatabaseType
    {
        /// <summary>
        /// 未选择数据库
        /// </summary>
        NONE,
        /// <summary>
        /// ACCESS数据库
        /// </summary>
        ACCESS,
        /// <summary>
        /// MYSQL数据库（需要引用[MySql.Data]NuGet项目或[MySql.Data.dll]文件）
        /// </summary>
        MYSQL,
        /// <summary>
        /// ORACLE数据库（需要引用[Oracle.ManagedDataAccess]NuGet项目或[Oracle.ManagedDataAccess.dll]文件）
        /// </summary>
        ORACLE,
        /// <summary>
        /// SQL SERVER数据库
        /// </summary>
        SQLSERVER,
    }
}
