using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

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
        /// <summary>
        /// 下载代理页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string DownloadProxyPage(string url)
        {
            try
            {
                //IsInit为true第一次运行时，抓取所有的,代理网站一般也有防爬虫的策略，所以也要使用代理
                if(ConfigHelper.GetIsInit())
                {
                    return DownloadWithProxy(url);
                }
                else
                {
                    //不是初始化每次只抓取第一页，不需要用代理
                    return HttpHelper.DownloadHtml(url, null, TimeOut);
                }
            }
            catch
            {
               
            }
            return "";
        }
        /// <summary>
        /// 抓取页面时使用代理
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadWithProxy(string url)
        {
            try
            {
                for (var i = 0; i < 3; i++)
                {
                    WebProxy webProxy = null;

                    var service = new ProxyService();
                    Proxy proxy = service.GetProxy();
                    if (proxy != null)
                    {
                        webProxy = new WebProxy(proxy.Adress + ":" + proxy.Port);
                    }

                    var html = HttpHelper.DownloadHtml(url, webProxy, TimeOut);
                    return html;
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        protected bool CheckHtml(string html,string url,string name)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                LogHelper.LogError(name+"，无法获取页面:：" + url);
                return false;
            }
            return true;
        }
    }
}
