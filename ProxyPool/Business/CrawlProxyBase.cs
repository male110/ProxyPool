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
            TimeOut = ConfigHelper.GetCrawlTimeOut();
        }
        /// <summary>
        /// 开始抓取代理
        /// </summary>
       public abstract void Start();
        protected bool VerifyProxy(Proxy p)
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
        /// <summary>
        /// 验证代理的有效性并保存
        /// </summary>
        /// <param name="lstProxy"></param>
        protected void VerifyAndSave(List<Proxy> listProxy)
        {
            if (listProxy == null || listProxy.Count == 0)
                return;
            //先过滤掉已存在的，然后再验证有效性
            var service = new ProxyService();
            listProxy = service.FilterExist(listProxy);
            //验证有效性
            List<Proxy> verifyedProxy = new List<Proxy>();
            var lockObj = new Object();
            Parallel.ForEach(listProxy, item =>
            {
                if (VerifyProxy(item))
                {
                    lock (lockObj)
                    {
                        verifyedProxy.Add(item);
                    }

                }
            });
            service.Add(verifyedProxy);
        }
        protected string UrlCombin(string url,string path)
        {
            return url.TrimEnd('/') + "/" + path.TrimStart('/');
        }
        /// <summary>
        /// 下载页面的超时时间，单位秒
        /// </summary>
        protected int TimeOut { get; set; }
    }
}
