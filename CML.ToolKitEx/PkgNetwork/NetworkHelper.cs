using CML.PkgResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace CML.PkgNetwork
{
    /// <summary>
    /// 网络帮助类
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// 获取HTML代码
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>HTML代码</returns>
        public static TResult<string> GetHtmlCode(MWebRequest webRequest, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            try
            {
                TResult<Stream> rltStream = GetWebStream(webRequest, requestCookie, out responseCookie);

                if (!rltStream.IsSuccess)
                {
                    responseCookie = null;
                    return new TResult<string>(default, rltStream.ErrorMessage);
                }

                //接收结果
                string result = string.Empty;

                //判断是否限速
                MTransSpeed transmissionSpeed = webRequest.DownloadSpeed;
                if (transmissionSpeed.EnableLimit)
                {
                    //数据列表
                    List<byte> lstBytes = new List<byte>();

                    //缓存字节数
                    int bufferSize = transmissionSpeed.Speed * (int)Math.Pow(2, Convert.ToInt32(transmissionSpeed.Unit));
                    //缓存
                    byte[] btBuffer = new byte[bufferSize];

                    //读取的字节数
                    int readSize = rltStream.Result.Read(btBuffer, 0, bufferSize);
                    while (readSize > 0)
                    {
                        //从服务器读取
                        rltStream.Result.Read(btBuffer, 0, readSize);
                        //记入列表
                        lstBytes.AddRange(btBuffer);

                        readSize = rltStream.Result.Read(btBuffer, 0, bufferSize);

                        //延时
                        if (readSize > 0)
                        {
                            Thread.Sleep(transmissionSpeed.Delay);
                        }
                    }

                    //转化为字符串
                    result = webRequest.Encoding.GetString(lstBytes.ToArray());
                }
                else
                {
                    using (StreamReader streamReader = new StreamReader(rltStream.Result, webRequest.Encoding))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }

                rltStream.Result.Close();

                return new TResult<string>(result);
            }
            catch (Exception ex)
            {
                responseCookie = null;
                return new TResult<string>(default, ex.Message);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <param name="returnMsg">[OUT]返回消息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(MWebRequest webRequest, string filePath, CookieContainer requestCookie, out CookieContainer responseCookie, out string returnMsg)
        {
            //本地文件
            FileInfo file = new FileInfo(filePath);

            if (!file.Exists)
            {
                responseCookie = null;
                returnMsg = "";
                return new TResult<bool>(false, "本地文件不存在！");
            }
            else
            {
                try
                {
                    // 随机分隔线
                    string boundary = "CML.ToolKit.NetworkEx." + DateTime.Now.Ticks.ToString("X");

                    //请求头
                    string strPostHeader =
                         $"--{boundary}\r\n" +
                         $"Content-Disposition: form-data; name=\"file\"; filename=\"{file.Name}\"\r\n" +
                         $"Content-Type: application/octet-stream\r\n\r\n";

                    //请求尾
                    string strPostEnder = $"\r\n--{boundary}--\r\n";

                    //请求数据
                    List<byte> bytePostData = new List<byte>();
                    bytePostData.AddRange(Encoding.ASCII.GetBytes(strPostHeader));
                    bytePostData.AddRange(File.ReadAllBytes(file.FullName));
                    bytePostData.AddRange(Encoding.ASCII.GetBytes(strPostEnder));

                    //构造请求信息模型
                    webRequest.ContentType = $"multipart/form-data;boundary={boundary}";
                    webRequest.PostBytes = bytePostData.ToArray();
                    webRequest.Method = ERequestMethod.POST;

                    TResult<Stream> rltStream = GetWebStream(webRequest, requestCookie, out responseCookie);
                    if (!rltStream.IsSuccess)
                    {
                        responseCookie = null;
                        returnMsg = "";
                        return new TResult<bool>(default, rltStream.ErrorMessage);
                    }

                    using (StreamReader streamReader = new StreamReader(rltStream.Result, webRequest.Encoding))
                    {
                        returnMsg = streamReader.ReadToEnd();
                    }

                    return new TResult<bool>(true);
                }
                catch (Exception ex)
                {
                    responseCookie = null;
                    returnMsg = "";
                    return new TResult<bool>(default, ex.Message);
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> DownloadFile(MWebRequest webRequest, string savePath, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            try
            {
                TResult<Stream> rltStream = GetWebStream(webRequest, requestCookie, out responseCookie);

                if (!rltStream.IsSuccess)
                {
                    return new TResult<bool>(default, rltStream.ErrorMessage);
                }

                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                {
                    MTransSpeed transmissionSpeed = webRequest.DownloadSpeed;
                    if (transmissionSpeed.EnableLimit)
                    {
                        //缓存字节数
                        int bufferSize = transmissionSpeed.Speed * (int)Math.Pow(2, Convert.ToInt32(transmissionSpeed.Unit));
                        //缓存
                        byte[] btBuffer = new byte[bufferSize];

                        //读取的字节数
                        int readSize = rltStream.Result.Read(btBuffer, 0, bufferSize);
                        while (readSize > 0)
                        {
                            //写入服务器
                            fileStream.Write(btBuffer, 0, readSize);

                            readSize = rltStream.Result.Read(btBuffer, 0, bufferSize);

                            //延时
                            if (readSize > 0)
                            {
                                Thread.Sleep(transmissionSpeed.Delay);
                            }
                        }
                    }
                    else
                    {
                        rltStream.Result.CopyTo(fileStream);
                    }
                }

                return new TResult<bool>(true);
            }
            catch (Exception ex)
            {
                responseCookie = null;
                return new TResult<bool>(default, ex.Message);
            }
        }

        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>数据流</returns>
        public static TResult<Stream> GetWebStream(MWebRequest webRequest, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            try
            {
                //检查输入URL
                if (!(WebRequest.Create(webRequest.RequestUrl) is HttpWebRequest httpWebRequest))
                {
                    responseCookie = null;
                    return new TResult<Stream>(default, "URL不规范！");
                }
                else
                {
                    //初始化请求模型
                    httpWebRequest.KeepAlive = webRequest.KeepAlive;
                    httpWebRequest.AllowAutoRedirect = webRequest.AllowAutoRedirect;

                    if (webRequest.Proxy.Enable)
                    {
                        httpWebRequest.Proxy = new WebProxy(webRequest.Proxy.Host, webRequest.Proxy.Port);
                    }
                    httpWebRequest.Timeout = webRequest.TimeOut;
                    httpWebRequest.Method = webRequest.Method.ToString();
                    httpWebRequest.ProtocolVersion = webRequest.ProtocolVersion;

                    if (!string.IsNullOrEmpty(webRequest.Host))
                    {
                        httpWebRequest.Host = webRequest.Host;
                    }
                    if (!string.IsNullOrEmpty(webRequest.Accept))
                    {
                        httpWebRequest.Accept = webRequest.Accept;
                    }
                    if (!string.IsNullOrEmpty(webRequest.Referer))
                    {
                        httpWebRequest.Referer = webRequest.Referer;
                    }
                    if (!string.IsNullOrEmpty(webRequest.UserAgent))
                    {
                        httpWebRequest.UserAgent = webRequest.UserAgent;
                    }
                    if (!string.IsNullOrEmpty(webRequest.ContentType))
                    {
                        httpWebRequest.ContentType = webRequest.ContentType;
                    }
                    if (!string.IsNullOrEmpty(webRequest.ContentType))
                    {
                        httpWebRequest.ContentType = webRequest.ContentType;
                    }
                    foreach (string key in webRequest.Headers.Keys)
                    {
                        httpWebRequest.Headers[key] = webRequest.Headers[key];
                    }
                    if (!string.IsNullOrEmpty(webRequest.Cookie))
                    {
                        httpWebRequest.Headers["Cookie"] = webRequest.Cookie;
                    }

                    httpWebRequest.CookieContainer = requestCookie;

                    //判断请求方式
                    if (webRequest.Method == ERequestMethod.POST)
                    {
                        byte[] postData;
                        //优先处理 PostBytes
                        if (webRequest.PostBytes == null)
                        {
                            StringBuilder postString = new StringBuilder();

                            //再处理 PostString
                            if (string.IsNullOrEmpty(webRequest.PostString))
                            {
                                foreach (string key in webRequest.PostDictionary.Keys)
                                {
                                    if (!string.IsNullOrEmpty(key))
                                    {
                                        postString.Append($"{key}={ webRequest.PostDictionary[key]}&");
                                    }
                                }
                            }
                            else
                            {
                                postString.Append(webRequest.PostString);
                            }

                            postData = Encoding.UTF8.GetBytes(postString.ToString().TrimEnd('&'));
                        }
                        else
                        {
                            postData = webRequest.PostBytes;
                        }

                        httpWebRequest.ContentLength = postData.Length;
                        using (Stream requestStream = httpWebRequest.GetRequestStream())
                        {
                            MTransSpeed transmissionSpeed = webRequest.UploadSpeed;
                            if (transmissionSpeed.EnableLimit)
                            {
                                //单次发送字节数
                                int bufferSize = transmissionSpeed.Speed * (int)Math.Pow(2, Convert.ToInt32(transmissionSpeed.Unit));

                                //发送数据字节数小于限速单次发送字节数
                                if (postData.Length <= bufferSize)
                                {
                                    requestStream.Write(postData, 0, postData.Length);
                                }
                                else
                                {
                                    //计算传输次数
                                    int count = postData.Length / bufferSize;
                                    //发送数据
                                    for (int i = 0; i < count; i++)
                                    {
                                        requestStream.Write(postData, i * bufferSize, bufferSize);

                                        //延时（非最后一次）
                                        if (i != count - 1)
                                        {
                                            Thread.Sleep(transmissionSpeed.Delay);
                                        }
                                    }

                                    //补偿剩余字节
                                    if (bufferSize * count != postData.Length)
                                    {
                                        Thread.Sleep(transmissionSpeed.Delay);
                                        requestStream.Write(postData, bufferSize * count, postData.Length - bufferSize * count);
                                    }
                                }
                            }
                            else
                            {
                                requestStream.Write(postData, 0, postData.Length);
                            }

                            requestStream.Close();
                        }
                    }

                    //获取远程响应
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        responseCookie = httpWebRequest.CookieContainer;
                        return new TResult<Stream>(httpWebResponse.GetResponseStream());
                    }
                    else
                    {
                        responseCookie = null;
                        return new TResult<Stream>(default, $"[{(int)httpWebResponse.StatusCode}:{httpWebResponse.StatusCode}]数据流请求错误！");
                    }
                }
            }
            catch (Exception ex)
            {
                responseCookie = null;
                return new TResult<Stream>(default, ex.Message);
            }
        }
    }

    /// <summary>
    /// 网络帮助类（扩展方法）
    /// </summary>
    public static class NetworkHelperE
    {
        #region 获取HTML代码
        /// <summary>
        /// 获取HTML代码
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <returns>HTML代码</returns>
        public static TResult<string> GetHtmlCode(this MWebRequest webRequest)
        {
            return NetworkHelper.GetHtmlCode(webRequest, null, out CookieContainer _);
        }

        /// <summary>
        /// 获取HTML代码
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <returns>HTML代码</returns>
        public static TResult<string> GetHtmlCode(this MWebRequest webRequest, CookieContainer requestCookie)
        {
            return NetworkHelper.GetHtmlCode(webRequest, requestCookie, out CookieContainer _);
        }

        /// <summary>
        /// 获取HTML代码
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>HTML代码</returns>
        public static TResult<string> GetHtmlCode(this MWebRequest webRequest, out CookieContainer responseCookie)
        {
            return NetworkHelper.GetHtmlCode(webRequest, null, out responseCookie);
        }

        /// <summary>
        /// 获取HTML代码
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>HTML代码</returns>
        public static TResult<string> GetHtmlCode(this MWebRequest webRequest, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            return NetworkHelper.GetHtmlCode(webRequest, requestCookie, out responseCookie);
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, null, out _, out _);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="returnMsg">[OUT]返回消息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, out string returnMsg)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, null, out _, out returnMsg);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, CookieContainer requestCookie)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, requestCookie, out _, out _);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="returnMsg">[OUT]返回消息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, CookieContainer requestCookie, out string returnMsg)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, requestCookie, out _, out returnMsg);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, out CookieContainer responseCookie)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, null, out responseCookie, out _);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <param name="returnMsg">[OUT]返回消息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, out CookieContainer responseCookie, out string returnMsg)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, null, out responseCookie, out returnMsg);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <param name="returnMsg">[OUT]返回消息</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> UploadFile(this MWebRequest webRequest, string filePath, CookieContainer requestCookie, out CookieContainer responseCookie, out string returnMsg)
        {
            return NetworkHelper.UploadFile(webRequest, filePath, requestCookie, out responseCookie, out returnMsg);
        }
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="savePath">保存路径</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> DownloadFile(this MWebRequest webRequest, string savePath)
        {
            return NetworkHelper.DownloadFile(webRequest, savePath, null, out CookieContainer _);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> DownloadFile(this MWebRequest webRequest, string savePath, CookieContainer requestCookie)
        {
            return NetworkHelper.DownloadFile(webRequest, savePath, requestCookie, out CookieContainer _);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> DownloadFile(this MWebRequest webRequest, string savePath, out CookieContainer responseCookie)
        {
            return NetworkHelper.DownloadFile(webRequest, savePath, null, out responseCookie);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>执行结果</returns>
        public static TResult<bool> DownloadFile(this MWebRequest webRequest, string savePath, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            return NetworkHelper.DownloadFile(webRequest, savePath, requestCookie, out responseCookie);
        }
        #endregion

        #region 获取数据流
        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <returns>数据流</returns>
        public static TResult<Stream> GetWebStream(this MWebRequest webRequest)
        {
            return NetworkHelper.GetWebStream(webRequest, null, out CookieContainer _);
        }

        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <returns>数据流</returns>
        public static TResult<Stream> GetWebStream(this MWebRequest webRequest, CookieContainer requestCookie)
        {
            return NetworkHelper.GetWebStream(webRequest, requestCookie, out CookieContainer _);
        }

        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>数据流</returns>
        public static TResult<Stream> GetWebStream(this MWebRequest webRequest, out CookieContainer responseCookie)
        {
            return NetworkHelper.GetWebStream(webRequest, null, out responseCookie);
        }

        /// <summary>
        /// 获取数据流
        /// </summary>
        /// <param name="webRequest">WEB请求信息</param>
        /// <param name="requestCookie">请求Cookie</param>
        /// <param name="responseCookie">[OUT]响应Cookie</param>
        /// <returns>数据流</returns>
        public static TResult<Stream> GetWebStream(this MWebRequest webRequest, CookieContainer requestCookie, out CookieContainer responseCookie)
        {
            return NetworkHelper.GetWebStream(webRequest, requestCookie, out responseCookie);
        }
        #endregion
    }
}
