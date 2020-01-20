namespace CML.PkgNetwork
{
    /// <summary>
    /// 代理
    /// </summary>
    public class MProxy
    {
        /// <summary>
        /// 是否启用代理
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 默认构造函数（不启用代理）
        /// </summary>
        public MProxy()
        {
            Enable = false;
        }

        /// <summary>
        /// 构造函数（启用代理）
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public MProxy(string host, int port)
        {
            Host = host;
            Port = port;
            Enable = true;
        }
    }
}
