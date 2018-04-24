using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;

namespace ProxyPool
{
    public class PoolSpider
    {
        public DateTime LastExecuteTime { get; set; }
        public void Run()
        {
            List<Task> taskList = new List<Task>() {
                /*new Task(new CrawlXiaoShu().Start),
                new Task(new CrawlXiciDaili().Start),*/
                new Task(new CrawlCodeBusy().Start)
            };

            while (true)
            {
                try
                {
                    var s = DateTime.Now - LastExecuteTime;
                    if (s.TotalMinutes < ConfigHelper.GetCrawInterval())
                    {
                        //每5分钟更新一次，如果间隔不足5分钟sleep
                        Thread.Sleep(1000 * 3);
                        continue;
                    }
                    foreach (Task t in taskList)
                    {
                        t.Start();
                    }
                    Task.WaitAll(taskList.ToArray());
                    LastExecuteTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("抓取代理时出错：" + ex);
                }
            }
        }

        /*
        #region 无忧代理
        public void Data5u()//无忧代理
        {
            try
            {
                var service = new ProxyService();
                List<string> urlList = new List<string>(){//这里只取国内的ip
                    "http://www.data5u.com/free/gngn/index.shtml",//同办高匿
                    "http://www.data5u.com/free/gnpt/index.shtml",//同内普通
                };
               foreach(var url in urlList)
                {
                    var proxyList = new List<Proxy>();
                    string html =HttpHelper.DownloadHtml(url, null,5);
                    string xpath = "/html/body/div[@class='wlist']/ul/li/ul[@class='l2']";
                    HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                    HtmlNodeCollection collection = node.SelectNodes(xpath);

                    Parallel.ForEach(collection, (item, state) => {
                        var rowNodes = item.SelectNodes("span/li");
                        if (rowNodes == null || rowNodes.Count == 0)
                            return;
                        Proxy proxy = new Proxy();
                        proxy.Adress = rowNodes[0].InnerHtml;
                        proxy.Port = int.Parse(rowNodes[1].InnerHtml);
                        proxy.Source = url;
                        if (VerifyProxy(proxy))
                        {
                            proxyList.Add(proxy);
                        }
                    });
                    service.Add(proxyList);
                }
                
            }
            catch(Exception ex)
            {
                LogHelper.LogError("抓取快代理时出错：" + ex);
            }
           
        }
        #endregion     */
    }
}
