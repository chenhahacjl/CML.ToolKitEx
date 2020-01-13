using System.Data;

namespace CML.PkgDatabase
{
    /// <summary>
    /// 数据传输参数模型
    /// </summary>
    public class MDataParameter
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DbType DataType { get; set; }

        /// <summary>
        /// 数据传输参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="type">数据类型</param>
        public MDataParameter(string name, object value, DbType type)
        {
            Name = name;
            Value = value;
            DataType = type;
        }
    }
}
