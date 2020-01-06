using System;
using System.ComponentModel;
using System.Reflection;

namespace CML.PkgResult
{
    /// <summary>
    /// 泛型操作结果类
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class TResult<T> : TResultBase
    {
        #region 结果数据
        /// <summary>
        /// 泛型结果数据
        /// </summary>
        public T Result { get; } = default;
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得异常信息字符串
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns></returns>
        private static string GetExceptionString(Exception ex)
        {
            return
                $"*************************异常详细信息*************************\r\n" +
                $"【发生时间】 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\r\n" +
                $"【异常类型】 {ex.GetType().Name}\r\n" +
                $"【异常方法】 {ex.TargetSite}\r\n" +
                $"【异常信息】 {ex.Message}\r\n" +
                $"【堆栈调用】 {ex.StackTrace}\r\n" +
                $"**************************************************************";
        }

        /// <summary>
        /// 获取枚举数值
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns>返回枚举的描述</returns>
        private static int GetEnumNumber(Enum en)
        {
            return en == null ? -1 : Convert.ToInt32(en);
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns>返回枚举的描述</returns>
        private static string GetEnumDescription(Enum en)
        {
            if (en != null)
            {
                //获取成员
                MemberInfo[] memberInfos = en.GetType().GetMember(en.ToString());

                if (memberInfos != null && memberInfos.Length > 0)
                {
                    //获取描述特性
                    if (memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attribute && attribute.Length > 0)
                    {
                        //返回当前描述
                        return attribute[0].Description;
                    }
                }

                return en.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造成功操作结果
        /// </summary>
        /// <param name="result">结果数据</param>
        public TResult(T result) : base()
        {
            Result = result;
        }

        /// <summary>
        /// 构造失败操作结果（错误代码为-1）
        /// </summary>
        /// <param name="result">结果数据</param>
        /// <param name="errMsg">错误描述</param>
        public TResult(T result, string errMsg) : base(-1, errMsg.Trim())
        {
            Result = result;
        }

        /// <summary>
        /// 构造失败操作结果（错误代码为-1）
        /// </summary>
        /// <param name="result">结果数据</param>
        /// <param name="exception">异常信息对象</param>
        public TResult(T result, Exception exception) : base(-1, GetExceptionString(exception))
        {
            Result = result;
        }

        /// <summary>
        /// 构造操作结果
        /// </summary>
        /// <param name="result">结果数据</param>
        /// <param name="errCode">错误代码(0代表调用成功)</param>
        /// <param name="errMsg">错误描述</param>
        public TResult(T result, int errCode, string errMsg) : base(errCode, errMsg?.Trim())
        {
            Result = result;
        }

        /// <summary>
        /// 构造操作结果
        /// </summary>
        /// <param name="result">结果数据</param>
        /// <param name="errEnum">描述枚举（取数值与描述）</param>
        public TResult(T result, Enum errEnum) : base(GetEnumNumber(errEnum), GetEnumDescription(errEnum))
        {
            Result = result;
        }
        #endregion

        #region 重写、重载
        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{ErrorCode}]{ErrorMessage}";
        }

        /// <summary>
        /// 重载!操作符
        /// </summary>
        /// <param name="result">操作结果</param>
        /// <returns></returns>
        public static bool operator !(TResult<T> result)
        {
            return !result.IsSuccess;
        }
        #endregion
    }
}
