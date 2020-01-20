using CML.PkgDingTalk.Model;
using System.Collections.Generic;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// 整体跳转ActionCard类型消息
    /// </summary>
    public class MSingleActionCardMsg : MSendMsgBase
    {
        /// <summary>
        /// 消息类型（actionCard）
        /// </summary>
        public override string msgtype => "actionCard";

        /// <summary>
        /// actionCard设置
        /// </summary>
        public ActionCardSetting actionCard { get; set; } = new ActionCardSetting();

        /// <summary>
        /// actionCard设置
        /// </summary>
        public class ActionCardSetting
        {
            /// <summary>
            /// 首屏会话透出的展示内容
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// markdown格式的消息
            /// </summary>
            public string text { get; set; }

            /// <summary>
            /// 单个按钮的方案。
            /// </summary>
            public string singleTitle { get; set; }

            /// <summary>
            /// 点击singleTitle按钮触发的URL
            /// </summary>
            public string singleURL { get; set; }

            /// <summary>
            /// 0-正常发消息者头像，1-隐藏发消息者头像
            /// </summary>
            public string hideAvatar { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public ActionCardSetting()
            {
                title = "ActionCardTitle";
                text = "ActionCardText";
                singleTitle = "SingleTitleTitle";
                singleURL = MessageUrl;
                hideAvatar = "0";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            /// <param name="_singleTitle">单个按钮的方案</param>
            /// <param name="_singleURL">点击singleTitle按钮触发的URL</param>
            public ActionCardSetting(string _title, string _text, string _singleTitle, string _singleURL)
            {
                title = string.IsNullOrEmpty(_title) ? "ActionCardTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "ActionCardText" : _text;
                singleTitle = string.IsNullOrEmpty(_singleTitle) ? "SingleTitleTitle" : _singleTitle;
                singleURL = string.IsNullOrEmpty(_singleURL) ? MessageUrl : _singleURL;
                hideAvatar = "0";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            /// <param name="_singleTitle">单个按钮的方案</param>
            /// <param name="_singleURL">点击singleTitle按钮触发的URL</param>
            /// <param name="_hideAvatar">0-正常发消息者头像，1-隐藏发消息者头像</param>
            public ActionCardSetting(string _title, string _text, string _singleTitle, string _singleURL, string _hideAvatar)
            {
                title = string.IsNullOrEmpty(_title) ? "ActionCardTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "ActionCardText" : _text;
                singleTitle = string.IsNullOrEmpty(_singleTitle) ? "SingleTitleTitle" : _singleTitle;
                singleURL = string.IsNullOrEmpty(_singleURL) ? MessageUrl : _singleURL;
                hideAvatar = new List<string> { "0", "1" }.Contains(_hideAvatar) ? _hideAvatar : "0";
            }
        }
    }
}
