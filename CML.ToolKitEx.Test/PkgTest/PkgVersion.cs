using CML.PkgVersion;
using System.Reflection;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 版本控制工具包测试
    /// </summary>
    internal class PkgVersion : PkgTestBase
    {
        /// <summary>
        /// 测试类名
        /// </summary>
        public override string ClassName => "PkgVersion";

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
            //实例化版本测试类
            VersionBase version = new VersionTestClass();

            PrintLog(MsgType.Info, "主版本号: " + version.VerMain);
            PrintLog(MsgType.Info, "研发版本号: " + version.VerDev);
            PrintLog(MsgType.Info, "更新时间: " + version.VerDate);
            PrintLog(MsgType.Info, "版本信息: " + version.GetVersionInfo());

            return true;
        }

        /// <summary>
        /// 版本测试类
        /// </summary>
        private class VersionTestClass : VersionBase
        {
            #region 版本信息
            /// <summary>
            /// 主版本号
            /// </summary>
            public override string VerMain => "1.0";
            /// <summary>
            /// 研发版本号
            /// </summary>
            public override string VerDev => "96Y096R096";
            /// <summary>
            /// 更新时间
            /// </summary>
            public override string VerDate => "1996年01月25日 00:00";
            /// <summary>
            /// 当前程序集 
            /// </summary>
            protected override Assembly RunAssembly => null;
            #endregion

            #region 公共方法
            /// <summary>
            /// 获得版本信息
            /// </summary>
            /// <returns>版本信息</returns>
            public string GetVersionInfo()
            {
                return "版本测试类版本信息";
            }
            #endregion
        }
    }
}
