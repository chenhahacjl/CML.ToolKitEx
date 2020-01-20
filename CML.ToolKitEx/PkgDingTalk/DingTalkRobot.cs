using CML.PkgDingTalk.Model;
using CML.PkgNetwork;
using CML.PkgResult;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace CML.PkgDingTalk
{
    /// <summary>
    /// 钉钉群自定义机器人操作类
    /// </summary>
    public class DingTalkRobot
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DingTalkRobot()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        public DingTalkRobot(string accessToken) : this(accessToken, string.Empty)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        /// <param name="secret">Secret</param>
        public DingTalkRobot(string accessToken, string secret)
        {
            AccessToken = accessToken;
            Secret = secret;
        }

        /// <summary>
        /// 序列化实体
        /// </summary>
        /// <param name="obj">实体</param>
        /// <returns>字符串</returns>
        private string SerializeObject(object obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            new JavaScriptSerializer().Serialize(obj, stringBuilder);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 反序列化字符串
        /// </summary>
        /// <param name="json">字符串</param>
        /// <returns>实体</returns>
        private T DeserializeObject<T>(string json)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            try
            {
                T a = javaScriptSerializer.Deserialize<T>(json);
                return a;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendMessage">消息内容</param>
        /// <returns>发送结果</returns>
        public TResult<bool> SendMessage(MSendMsgBase sendMessage)
        {
            MWebRequest webRequest = new MWebRequest()
            {
                PostString = SerializeObject(sendMessage),
                Method = ERequestMethod.POST,
                ContentType = "application/json",
                RequestUrl = $"https://oapi.dingtalk.com/robot/send?access_token={AccessToken}",
            };

            if (!string.IsNullOrEmpty(Secret))
            {
                try
                {
                    TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    string timestamp = Convert.ToInt64(timeSpan.Ticks / 10000).ToString();
                    webRequest.RequestUrl += $"&timestamp={timestamp}";

                    using (HMACSHA256 sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(Secret)))
                    {
                        byte[] sha256Byte = sha256.ComputeHash(Encoding.UTF8.GetBytes(timestamp + "\n" + Secret));
                        string sign = HttpUtility.UrlEncode(Convert.ToBase64String(sha256Byte));

                        webRequest.RequestUrl += $"&sign={sign}";
                    }
                }
                catch (Exception ex)
                {
                    return new TResult<bool>(false, "发送错误: " + ex.Message);
                }
            }

            TResult<string> rltJson = webRequest.GetHtmlCode();
            if (!rltJson.IsSuccess)
            {
                return new TResult<bool>(false, "发送错误: " + rltJson.ErrorMessage);
            }

            try
            {
                MReceiveMsg rltMsg = DeserializeObject<MReceiveMsg>(rltJson.Result);
                if (rltMsg.errcode == 0)
                {
                    return new TResult<bool>(true);
                }
                else
                {
                    return new TResult<bool>(false, "发送失败: " + rltMsg.errmsg);
                }
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, "发送错误: " + ex.Message);
            }
        }
    }
}
