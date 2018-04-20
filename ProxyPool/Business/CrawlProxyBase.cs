using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool
{
    public abstract class CrawlProxyBase
    {
        public CrawlProxyBase()
        {
            TimeOut = 5;
        }
        /// <summary>
        /// 开始抓取代理
        /// </summary>
       public abstract void Start();
        public bool VerifyProxy(Proxy p)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Adress) || p.Port == 0)
            {
                return false;
            }
            var state = ProxyVerify.IsAvailable(p);
            if (state.IsAvailable)
            {
                p.LastVerifyDate = DateTime.Now;
                p.Speed = state.Speed;
            }
            return state.IsAvailable;
        }
        public string UrlCombin(string url,string path)
        {
            return url.TrimEnd('/') + "/" + path.TrimStart('/');
        }
        /// <summary>
        /// 下载页面的超时时间，单位秒
        /// </summary>
        public int TimeOut { get; set; }
    }
}
