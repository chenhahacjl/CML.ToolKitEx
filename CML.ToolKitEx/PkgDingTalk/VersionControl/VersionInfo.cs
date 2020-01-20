using CML.PkgVersion;
using System.Reflection;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// 钉钉操作工具包版本信息
    /// </summary>
    public class VersionInfo : VersionBase
    {
        #region 版本信息
        /// <summary>
        /// 主版本号
        /// </summary>
        public override string VerMain => "1.0";
        /// <summary>
        /// 研发版本号
        /// </summary>
        public override string VerDev => "20Y001R001";
        /// <summary>
        /// 更新时间
        /// </summary>
        public override string VerDate => "2020年01月20日 16:00";
        /// <summary>
        /// 当前程序集 
        /// </summary>
        protected override Assembly RunAssembly => Assembly.GetExecutingAssembly();
        #endregion

        #region 公共方法
        /// <summary>
        /// 获得版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public string GetVersionInfo()
        {
            string filePath = "CML.PkgDingTalk.VersionControl.UpdateInfo.LOG";
            return base.GetVersionInfo(filePath);
        }
        #endregion
    }
}
