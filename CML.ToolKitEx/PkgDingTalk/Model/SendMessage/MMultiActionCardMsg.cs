using CML.PkgDingTalk.Model;
using System.Collections.Generic;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// 独立跳转ActionCard类型消息
    /// </summary>
    public class MMultiActionCardMsg : MSendMsgBase
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
            /// btns设置
            /// </summary>
            public ButtonSetting[] btns { get; set; }

            /// <summary>
            /// 0-按钮竖直排列，1-按钮横向排列
            /// </summary>
            public string btnOrientation { get; set; }

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
                btns = new ButtonSetting[] { new ButtonSetting() };
                btnOrientation = "0";
                hideAvatar = "0";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            /// <param name="_btns">btns设置</param>
            public ActionCardSetting(string _title, string _text, ButtonSetting[] _btns)
            {
                title = string.IsNullOrEmpty(_title) ? "ActionCardTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "ActionCardText" : _text;
                btns = _btns ?? new ButtonSetting[] { new ButtonSetting() };
                btnOrientation = "0";
                hideAvatar = "0";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            /// <param name="_btns">btns设置</param>
            /// <param name="_btnOrientation">0-按钮竖直排列，1-按钮横向排列</param>
            public ActionCardSetting(string _title, string _text, ButtonSetting[] _btns, string _btnOrientation)
            {
                title = string.IsNullOrEmpty(_title) ? "ActionCardTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "ActionCardText" : _text;
                btns = _btns ?? new ButtonSetting[] { new ButtonSetting() };
                btnOrientation = new List<string> { "0", "1" }.Contains(_btnOrientation) ? _btnOrientation : "0";
                hideAvatar = "0";
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">首屏会话透出的展示内容</param>
            /// <param name="_text">markdown格式的消息</param>
            /// <param name="_btns">btns设置</param>
            /// <param name="_btnOrientation">0-按钮竖直排列，1-按钮横向排列</param>
            /// <param name="_hideAvatar">0-正常发消息者头像，1-隐藏发消息者头像</param>
            public ActionCardSetting(string _title, string _text, ButtonSetting[] _btns, string _btnOrientation, string _hideAvatar)
            {
                title = string.IsNullOrEmpty(_title) ? "ActionCardTitle" : _title;
                text = string.IsNullOrEmpty(_text) ? "ActionCardText" : _text;
                btns = _btns ?? new ButtonSetting[] { new ButtonSetting() };
                btnOrientation = new List<string> { "0", "1" }.Contains(_btnOrientation) ? _btnOrientation : "0";
                hideAvatar = new List<string> { "0", "1" }.Contains(_hideAvatar) ? _hideAvatar : "0";
            }
        }

        /// <summary>
        /// btns设置
        /// </summary>
        public class ButtonSetting
        {
            /// <summary>
            /// 按钮方案
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 点击按钮触发的URL
            /// </summary>
            public string actionURL { get; set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            public ButtonSetting()
            {
                title = "ButtonSettingTitle";
                actionURL = MessageUrl;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="_title">按钮方案</param>
            /// <param name="_actionURL">点击按钮触发的URL</param>
            public ButtonSetting(string _title, string _actionURL)
            {
                title = string.IsNullOrEmpty(_title) ? "ButtonSettingTitle" : _title;
                actionURL = string.IsNullOrEmpty(_actionURL) ? MessageUrl : _actionURL;
            }
        }
    }
}
