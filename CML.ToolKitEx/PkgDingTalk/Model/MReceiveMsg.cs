using System;

namespace CML.PkgDingTalk.Model
{
    /// <summary>
    /// 接收消息
    /// </summary>
    [Serializable()]
    internal class MReceiveMsg
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string errmsg { get; set; }
    }
}
