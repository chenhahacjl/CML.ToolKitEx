using CML.PkgSingleton;

namespace CML.ToolKitEx.Test
{
    /// <summary>
    /// 单实例工具包测试
    /// </summary>
    internal class PkgSingleton : ToolkitTestBase
    {
        /// <summary>
        /// 测试类名
        /// </summary>
        public override string ClassName => "PkgSingleton";

        /// <summary>
        /// 更新时间
        /// </summary>
        public override string UpdateDate => new VersionInfo().VerDate;

        /// <summary>
        /// 版本信息
        /// </summary>
        public override string VersionInfo => $"{new VersionInfo().VerMain} => {new VersionInfo().VerDev}";

        /// <summary>
        /// 执行测试
        /// </summary>
        public override bool ExecuteTest()
        {
            TestClass testClass = new TestClass();

            bool rlt1 = TestClass.Instance.TestItem1 == testClass.TestItem1;
            PrintLog(rlt1 ? MsgType.Info : MsgType.FAIL, "TestItem1: " + TestClass.Instance.TestItem1);

            bool rlt2 = TestClass.Instance.TestItem2 == testClass.TestItem2;
            PrintLog(rlt2 ? MsgType.Info : MsgType.FAIL, "TestItem2: " + TestClass.Instance.TestItem2);

            return rlt1 && rlt2;
        }

        /// <summary>
        /// 测试类
        /// </summary>
        private class TestClass : SingletonBase<TestClass>
        {
            /// <summary>
            /// 测试项目1
            /// </summary>
            public string TestItem1 => "测试项目1";

            /// <summary>
            /// 测试项目2
            /// </summary>
            public string TestItem2 => "测试项目2";
        }
    }
}
