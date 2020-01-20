using CML.PkgDingTalk.Model;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// text类型消息
    /// </summary>
    public class MTextMsg : MSendMsgBase
    {
        /// <summary>
        /// 消息类型（text）
        /// </summary>
        public override string msgtype => "text";

        /// <summary>
        /// text设置    
        /// </summary>
        public TextSetting text { get; set; } = new TextSetting();

        /// <summary>
        /// @设置
        /// </summary>
        public AtSetting at { get; set; } = new AtSetting();

        /// <summary>
        /// text设置
        /// </summary>
        public class TextSetting
        {
            /// <summary>
            /// 消息内容
            /// </summary>
            public string content { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public TextSetting()
            {
                content = "TextMsgContent";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_content">消息类型</param>
            public TextSetting(string _content)
            {
                content = string.IsNullOrEmpty(_content) ? "TextMsgContent" : _content;
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
