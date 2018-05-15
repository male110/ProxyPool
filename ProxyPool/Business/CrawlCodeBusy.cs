using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProxyPool
{
    public class CrawlCodeBusy:CrawlProxyBase
    {
        public override void Start()
        {
            try
            {
                var url = "https://proxy.coderbusy.com/classical/isp/%e9%98%bf%e9%87%8c%e4%ba%91.aspx";
                var html = DownloadProxyPage(url);
                if (html == "")
                {
                    LogHelper.LogError("码农代理，无法获取页面" + url);
                    return;
                }
                var doc = new HtmlDocument();
                if (!CheckHtml(html, url, "码农代理"))
                {
                    return;
                }
                doc.LoadHtml(html);
                var rootNode = doc.DocumentNode;
                var totalPage = GetTotalPage(rootNode);
                for(var i=1;i<=totalPage;i++)
                {
                    var newPage = url + "?page=" + i;
                    if(i!=1)
                    {
                        html = DownloadProxyPage(newPage);
                        doc.LoadHtml(html);
                        rootNode = doc.DocumentNode;
                    }
                    var lines=rootNode.SelectNodes("//div[@id='site-app']//table[@class='table']/tbody/tr");
                    if (lines == null)
                        continue;
                    var listProxy = new List<Proxy>();
                    Parallel.ForEach(lines, item => {
                        try
                        {
                            var proxy = new Proxy();
                            proxy.Adress = item.SelectSingleNode("td[1]").InnerText.Trim();
                            int port = 0;
                            if (!int.TryParse(item.SelectSingleNode("td[3]").InnerText, out port))
                            {
                                //获取端口号时出错
                                LogHelper.LogError("码农代理，取端口号时出错。");
                                return;
                            }
                            proxy.Port = port;
                            proxy.Source = newPage;
                            listProxy.Add(proxy);
                        }
                        catch(Exception ex)
                        {
                            LogHelper.LogError("解析码农代理时出错：" + ex);
                        }
                    });
                    VerifyAndSave(listProxy);
                }
            }
            catch(Exception ex)
            {
                LogHelper.LogError("抓取码农代理时出错："+ex);
            }
        }

        /// <summary>
        /// 取总页数
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        private int GetTotalPage(HtmlNode rootNode)
        {
            try
            {
                //如果不是第一次抓取，只抓第一页，直接返回1
                if (!ConfigHelper.GetIsInit())
                    return 1;
                //获取总页数
                var pagesNode = rootNode.SelectNodes("//div[@class='card-footer']/nav/ul/li/a");
                if (pagesNode == null || pagesNode.Count <= 1)
                {
                    return 1;
                }
                //取最后一页的
                var lastPageUrl = pagesNode[pagesNode.Count - 1].Attributes["href"].Value;
                var index = lastPageUrl.IndexOf("=");
                var strPage = lastPageUrl.Substring(index + 1);
                var totalPage = ConvertHelper.ToInt(strPage, 1);
                return totalPage;
            }
            catch(Exception ex)
            {
                LogHelper.LogError("码农代理，取总页数时出错："+ex);
                return 1;
            }
        }
    }
}
