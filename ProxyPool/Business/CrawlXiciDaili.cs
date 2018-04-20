﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProxyPool
{
    public class CrawlXiciDaili:CrawlProxyBase
    {
        public override void Start()
        {
            LogHelper.LogMsg("》》》开始抓取西刺代理");
            try
            {
               
                var service = new ProxyService();
                List<string> list = new List<string>()
                {
                    "http://www.xicidaili.com/nt/",
                    "http://www.xicidaili.com/nn/",
                    "http://www.xicidaili.com/wn/",
                    "http://www.xicidaili.com/wt/"

                };
                var lockObj = new Object();
                foreach (var utlitem in list)
                {
                    string url = utlitem;
                    string html = HttpHelper.DownloadHtml(url, null, TimeOut);
                    if(string.IsNullOrWhiteSpace(html))
                    {
                        LogHelper.LogError("西刺代理，无法获取页面:"+url);
                        continue;
                    }
                    HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                   
                    //获取总页数
                    var totalPage = GetTotalPage(node);
                    for (var i = 1; i <= totalPage; i++)
                    {
                        var pageUrl = url + i;
                        //如果是第一页，已经取过不用再取
                        if (i != 1)
                        {
                            html = HttpHelper.DownloadHtml(pageUrl, null, TimeOut);
                            doc.LoadHtml(html);
                            node = doc.DocumentNode;
                        }
                        string xpathstring = "//table[@id='ip_list']/tr[@class]";
                        HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                        var proxyList = new List<Proxy>();
                        //提取代理
                        Parallel.ForEach(collection, item =>
                    {
                        Proxy proxy = new Proxy();
                        string xpath = "td[2]";
                        proxy.Adress = item.SelectSingleNode(xpath).InnerHtml.Trim();
                        xpath = "td[3]";
                        proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml.Trim());
                        proxy.Source = pageUrl;
                        if (VerifyProxy(proxy))
                        {
                            lock (lockObj)
                            {
                                proxyList.Add(proxy);
                            }
                        }
                    });

                        service.Add(proxyList);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("抓取西刺代理时出错：" + ex);
            }
            LogHelper.LogMsg("《《《西刺代理抓取完成");
        }
        /// <summary>
        /// 取总页数
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        private int GetTotalPage(HtmlNode rootNode)
        {
            //如果不是第一次抓取，只抓第一页，直接返回1
            if (!ConfigHelper.GetIsInit())
                return 1;
            //获取总页数
            var pages = rootNode.SelectNodes("//div[@class='pagination']/a");
            if(pages==null||pages.Count<=2)
            {
                return 1;
            }
            var totalPage =Convert.ToInt32(pages[pages.Count - 2].InnerText.Trim());
            return totalPage;
        }
      
}
}