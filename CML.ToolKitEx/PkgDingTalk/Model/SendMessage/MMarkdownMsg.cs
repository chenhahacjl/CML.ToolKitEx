using CML.PkgDingTalk.Model;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// markdown类型消息
    /// </summary>
    public class MMarkdownMsg : MSendMsgBase
    {
        /// <summary>
        /// 消息类型（markdown）
        /// </summary>
        public override string msgtype => "markdown";

        /// <summary>
        /// markdown设置    
        /// </summary>
        public MarkdownSetting markdown { get; set; } = new MarkdownSetting();

        /// <summary>
        /// @设置
        /// </summary>
        public AtSetting at { get; set; } = new AtSetting();

        /// <summary>
        /// markdown设置    
        /// </summary>
        public class MarkdownSetting
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
            /// 构造函数
            /// </summary>
            public MarkdownSetting()
            {
                title = "MarkdownMsgTitle";
                text = "MarkdownMsgText";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            public MarkdownSetting(string _title, string _text)
            {
                title = string.IsNullOrEmpty(_title) ? "MarkdownMsgTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "MarkdownMsgText" : _text;
            }
        }

        /// <summary>
        /// @设置
        /// </summary>
        public class AtSetting
        {
            /// <summary>
            /// 被@人的手机号(在content里添加@人的手机号)
            /// </summary>
            public string[] atMobiles { get; set; }
            /// <summary>
            /// @所有人时：true，否则为：false
            /// </summary>
            public bool isAtAll { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public AtSetting()
            {
                atMobiles = new string[0];
                isAtAll = false;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_isAtAll">@所有人时：true，否则为：false</param>
            public AtSetting(bool _isAtAll)
            {
                atMobiles = new string[0];
                isAtAll = _isAtAll;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_atMobiles">被@人的手机号(在content里添加@人的手机号)</param>
            /// <param name="_isAtAll">@所有人时：true，否则为：false</param>
            public AtSetting(string[] _atMobiles, bool _isAtAll)
            {
                atMobiles = _atMobiles;
                isAtAll = _isAtAll;
            }
        }
    }
}
