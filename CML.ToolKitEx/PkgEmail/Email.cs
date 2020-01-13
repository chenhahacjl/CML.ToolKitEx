using CML.PkgResult;
using System;
using System.Net;
using System.Net.Mail;

namespace CML.PkgEmail
{
    /// <summary>
    /// Email操作类
    /// </summary>
    public class Email
    {
        #region 公共属性
        /// <summary>
        /// 发送服务器信息
        /// </summary>
        public MServerInfo ServerInfo { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Email()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverInfo">发送服务器信息</param>
        public Email(MServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailInfo">邮件信息</param>
        /// <returns>执行结果</returns>
        public TResult<bool> SendEmail(MEmailInfo emailInfo)
        {
            return EmailS.SendEmail(ServerInfo, emailInfo);
        }
        #endregion
    }

    /// <summary>
    /// Email操作类（扩展方法）
    /// </summary>
    public static class EmailE
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="serverInfo">[THIS]服务器信息</param>
        /// <param name="emailInfo">邮件信息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> SendEmail(this MServerInfo serverInfo, MEmailInfo emailInfo)
        {
            return EmailS.SendEmail(serverInfo, emailInfo);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailInfo">[THIS]邮件信息</param>
        /// <param name="serverInfo">服务器信息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> SendEmail(this MEmailInfo emailInfo, MServerInfo serverInfo)
        {
            return EmailS.SendEmail(serverInfo, emailInfo);
        }
    }

    /// <summary>
    /// Email操作类（静态方法）
    /// </summary>
    public static class EmailS
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="serverInfo">服务器信息</param>
        /// <param name="emailInfo">邮件信息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> SendEmail(MServerInfo serverInfo, MEmailInfo emailInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(serverInfo?.SmtpHost))
                {
                    return new TResult<bool>(false, "请填写SMTP服务器！");
                }
                if (serverInfo.SmtpPort < 1 || serverInfo.SmtpPort > 65535)
                {
                    return new TResult<bool>(false, "SMTP服务器端口填写错误！");
                }
                if (emailInfo.ToEmail == null || emailInfo.ToEmail.Count == 0)
                {
                    return new TResult<bool>(false, "请填写收件人！");
                }

                //构造邮件
                MailMessage mailMsg = new MailMessage
                {
                    From = new MailAddress(emailInfo.FromEmail, emailInfo.FromName, emailInfo.Encoding),
                    Priority = emailInfo.MailPriority,
                    SubjectEncoding = emailInfo.Encoding,
                    BodyEncoding = emailInfo.Encoding,
                };

                //收件人
                emailInfo.ToEmail?.ForEach(item => mailMsg.To.Add(item));
                //抄送人
                emailInfo.CCEmail?.ForEach(item => mailMsg.CC.Add(item));
                //密送人
                emailInfo.BCCEmail?.ForEach(item => mailMsg.Bcc.Add(item));
                //邮件标题
                mailMsg.Subject = emailInfo.Subject;
                // 邮件正文
                mailMsg.Body = emailInfo.Body;
                //邮件正文是否是HTML格式
                mailMsg.IsBodyHtml = emailInfo.IsBodyHtml;
                //邮件附件
                emailInfo.AttachmentList?.ForEach(item => mailMsg.Attachments.Add(item));

                //构造SMTP协议
                SmtpClient smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = serverInfo.EnableSsl,
                    Host = serverInfo.SmtpHost,
                    Port = serverInfo.SmtpPort,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(serverInfo.SmtpUser, serverInfo.SmtpPwd)
                };

                //发送邮件
                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                return new TResult<bool>(false, ex.Message);
            }

            return new TResult<bool>(true);
        }
    }
}
