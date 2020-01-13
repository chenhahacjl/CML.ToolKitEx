using System;
using System.Security.Cryptography;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 工具包测试基础类
    /// 【请重载 ClassName 属性】
    /// 【请重载 UpdateDate 属性】
    /// 【请重载 VersionInfo 属性】
    /// 【请重载 UpdateInfo 属性】
    /// 【请重载 ExecuteTest() 方法】
    /// </summary>
    internal class PkgTestBase
    {
        /// <summary>
        /// 测试类名【请重载此属性】
        /// </summary>
        public virtual string ClassName => "请重载 PkgTestBase 类 ClassName 属性";

        /// <summary>
        /// 更新时间【请重载此属性】
        /// </summary>
        public virtual string UpdateDate => "请重载 PkgTestBase 类 UpdateDate 属性";

        /// <summary>
        /// 版本信息【请重载此属性】
        /// </summary>
        public virtual string VersionInfo => "请重载 PkgTestBase 类 VersionInfo 属性";

        /// <summary>
        /// 更新信息【请重载此属性】
        /// </summary>
        public virtual string UpdateInfo => "请重载 PkgTestBase 类 UpdateInfo 属性";

        /// <summary>
        /// 执行测试【请重载此方法】
        /// </summary>
        public virtual bool ExecuteTest()
        {
            PrintLog(MsgType.FAIL, $"请重载 PkgTestBase 类 ExecuteTest() 方法");
            return false;
        }

        /// <summary>
        /// 控制台打印日志
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="msg">消息内容</param>
        /// <param name="isNewline">是否换行[默认为TRUE]</param>
        protected void PrintLog(MsgType type, string msg, bool isNewline = true)
        {
            Console.ForegroundColor =
                type == MsgType.Info ? ConsoleColor.White :
                type == MsgType.PASS ? ConsoleColor.Green :
                type == MsgType.Warn ? ConsoleColor.Yellow :
                ConsoleColor.Red;
            Console.Write($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}]{(msg.Trim().Contains("\n") ? "\n" : "")}{msg.Trim()}");
            if (isNewline) { Console.WriteLine(); }
            Console.ResetColor();
        }

        /// <summary>
        /// 控制台打印消息
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="msg">消息内容</param>
        /// <param name="isNewline">是否换行[默认为TRUE]</param>
        protected void PrintMsg(MsgType type, string msg, bool isNewline = true)
        {
            Console.ForegroundColor =
                type == MsgType.Info ? ConsoleColor.White :
                type == MsgType.PASS ? ConsoleColor.Green :
                type == MsgType.Warn ? ConsoleColor.Yellow :
                ConsoleColor.Red;
            Console.Write(msg.Trim());
            if (isNewline) { Console.WriteLine(); }
            Console.ResetColor();
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">字符串的长度</param>
        ///<returns>随机字符串</returns>
        protected string GetRandomString(int length)
        {
            byte[] bts = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(bts);
            Random r = new Random(BitConverter.ToInt32(bts, 0));

            string result = null;
            string model = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            for (int i = 0; i < length; i++)
            {
                result += model.Substring(r.Next(0, model.Length - 1), 1);
            }

            return result;
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        protected enum MsgType
        {
            /// <summary>
            /// 信息
            /// </summary>
            Info,
            /// <summary>
            /// 通过
            /// </summary>
            PASS,
            /// <summary>
            /// 警告
            /// </summary>
            Warn,
            /// <summary>
            /// 失败
            /// </summary>
            FAIL
        }
    }
}
