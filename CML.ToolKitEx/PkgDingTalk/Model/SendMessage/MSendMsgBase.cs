using System;

namespace CML.PkgDingTalk.Model
{
    /// <summary>
    /// 发送消息基类
    /// </summary>
    [Serializable()]
    public class MSendMsgBase
    {
        /// <summary>
        /// 范例消息URL
        /// </summary>
        protected const string MessageUrl = "https://ding-doc.dingtalk.com/doc#/serverapi2/qf2nxq";
        /// <summary>
        /// 范例图片URL
        /// </summary>
        protected const string ImageUrl = "https://img.alicdn.com/tfs/TB1bB9QKpzqK1RjSZFoXXbfcXXa-576-96.png";

        /// <summary>
        /// 消息类型
        /// </summary>
        public virtual string msgtype => "sendMsg";
    }
}
