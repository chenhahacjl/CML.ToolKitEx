using CML.PkgResult;
using System;
using System.ComponentModel;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 泛型结果工具包测试
    /// </summary>
    internal class PkgResult : PkgTestBase
    {
        /// <summary>
        /// 测试类名
        /// </summary>
        public override string ClassName => "PkgResult";

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
            //测试结果
            TResult<string>[] arrRltTest = new TResult<string>[]
            {
                //构造函数1
                new TResult<string>("结果数值1"),
                //构造函数2
                new TResult<string>("结果数值2", TestEnum.Fail),
                //构造函数3
                new TResult<string>("结果数值3", new Exception("失败描述-Exception")),
                //构造函数4
                new TResult<string>("结果数值4", "失败描述-字符串"),
                //构造函数5
                new TResult<string>("结果数值5", 0, "失败描述-错误代码")
            };

            foreach (TResult<string> rltTest in arrRltTest)
            {
                PrintLog(MsgType.Info, $"[{rltTest.ErrorCode}][{rltTest.IsSuccess}]{rltTest.Result}@{rltTest.ErrorMessage}");
            }

            return true;
        }

        /// <summary>
        /// 测试枚举
        /// </summary>
        private enum TestEnum
        {
            /// <summary>
            /// 失败
            /// </summary>
            [Description("失败描述-Enum")]
            Fail = 1,
        }
    }
}
