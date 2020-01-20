using CML.PkgDingTalk.Model;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// link类型消息
    /// </summary>
    public class MLinkMsg : MSendMsgBase
    {
        /// <summary>
        /// 消息类型（link）
        /// </summary>
        public override string msgtype => "link";

        /// <summary>
        /// link设置
        /// </summary>
        public LinkSetting link { get; set; } = new LinkSetting();

        /// <summary>
        /// link设置
        /// </summary>
        public class LinkSetting
        {
            /// <summary>
            /// 消息标题
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 消息内容。如果太长只会部分展示
            /// </summary>
            public string text { get; set; }

            /// <summary>
            /// 点击消息跳转的URL
            /// </summary>
            public string messageUrl { get; set; }

            /// <summary>
            /// 图片URL
            /// </summary>
            public string picUrl { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LinkSetting()
            {
                title = "LinkMsgTitle";
                text = "LinkMsgText";
                messageUrl = MessageUrl;
                picUrl = string.Empty;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">消息标题</param>
            /// <param name="_text">消息内容。如果太长只会部分展示</param>
            /// <param name="_messageUrl">点击消息跳转的URL</param>
            public LinkSetting(string _title, string _text, string _messageUrl)
            {
                title = string.IsNullOrEmpty(_title) ? "LinkMsgTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "LinkMsgText" : _text;
                messageUrl = string.IsNullOrEmpty(_messageUrl) ? MessageUrl : _messageUrl;
                picUrl = string.Empty;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">消息标题</param>
            /// <param name="_text">消息内容。如果太长只会部分展示</param>
            /// <param name="_messageUrl">点击消息跳转的URL</param>
            /// <param name="_picUrl">图片URL</param>
            public LinkSetting(string _title, string _text, string _messageUrl, string _picUrl)
            {
                title = string.IsNullOrEmpty(_title) ? "LinkMsgTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "LinkMsgText" : _text;
                messageUrl = string.IsNullOrEmpty(_messageUrl) ? MessageUrl : _messageUrl;
                picUrl = _picUrl;
            }
        }
    }
}
