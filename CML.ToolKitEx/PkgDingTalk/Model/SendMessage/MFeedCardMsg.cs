using CML.PkgDingTalk.Model;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// FeedCard类型消息
    /// </summary>
    public class MFeedCardMsg : MSendMsgBase
    {
        /// <summary>
        /// 消息类型（feedCard）
        /// </summary>
        public override string msgtype => "feedCard";

        /// <summary>
        /// feedCard设置
        /// </summary>
        public FeedCardSetting feedCard { get; set; } = new FeedCardSetting();

        /// <summary>
        /// feedCard设置
        /// </summary>
        public class FeedCardSetting
        {
            /// <summary>
            /// links设置
            /// </summary>
            public LinksSetting[] links { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public FeedCardSetting()
            {
                links = new LinksSetting[] { new LinksSetting() };
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_links">links设置</param>
            public FeedCardSetting(LinksSetting[] _links)
            {
                links = _links ?? new LinksSetting[] { new LinksSetting() };
            }
        }

        /// <summary>
        /// links设置
        /// </summary>
        public class LinksSetting
        {
            /// <summary>
            /// 单条信息文本
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 点击单条信息到跳转链接
            /// </summary>
            public string messageURL { get; set; }

            /// <summary>
            /// 单条信息后面图片的URL
            /// </summary>
            public string picURL { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public LinksSetting()
            {
                title = "FeedCardTitle";
                messageURL = MessageUrl;
                picURL = ImageUrl;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">单条信息文本</param>
            /// <param name="_messageURL">点击单条信息到跳转链接</param>
            /// <param name="_picURL">单条信息后面图片的URL</param>
            public LinksSetting(string _title, string _messageURL, string _picURL)
            {
                title = string.IsNullOrEmpty(_title) ? "FeedCardTitle" : _title;
                messageURL = string.IsNullOrEmpty(_messageURL) ? MessageUrl : _messageURL;
                picURL = string.IsNullOrEmpty(_picURL) ? ImageUrl : _picURL;
            }
        }
    }
}
