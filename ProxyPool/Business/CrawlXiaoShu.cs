using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProxyPool
{
    /// <summary>
    /// 小舒代理http://www.xsdaili.com/
    /// </summary>
    public class CrawlXiaoShu : CrawlProxyBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            CrawlProxy();
        }

        private void CrawlProxy()
        {
            LogHelper.LogMsg("》》》开始抓取小舒代理");
            try
            {
                var urlList = this.GetUrls();
                Parallel.ForEach(urlList, url =>
                 {
                     var html = HttpHelper.DownloadHtml(url, null, TimeOut);
                     var doc = new HtmlDocument();
                     doc.LoadHtml(html);
                     var node = doc.DocumentNode.SelectSingleNode("//div[@class='cont']");
                     if (node == null)
                     {
                         LogHelper.LogError("获取小舒代理ip时出错，请检查网站结构是否有改动");
                         return;
                     }
                     var listProxy = new List<Proxy>();
                //代理Ip是以br分隔的所以直接取文本
                var strIp = node.InnerHtml.Replace("<br>", "\n").Replace(" <br />", "\n").Replace(" <br/>", "\n");
                     var lines = strIp.Split("\n");
                     Parallel.ForEach(lines, line =>
                     {
                         line = line.Trim();
                         if (string.IsNullOrWhiteSpace(line))
                         {
                             return;
                         }
                         var index = line.IndexOf('@');
                         if (index == -1)
                             return;
                         var ipWithPort = line.Substring(0, index);
                         index = ipWithPort.IndexOf(":");
                         if (index == -1)
                             return;
                         var proxy = new Proxy();
                         proxy.Adress = ipWithPort.Substring(0, index).Trim();
                         var strPort = ipWithPort.Substring(index + 1).Trim();
                         int port = 0;
                         if (!int.TryParse(strPort, out port))
                         {
                             LogHelper.LogError("小舒代理，解析端口号时出错：" + ipWithPort);
                             return;
                         }
                         proxy.Port = port;
                         proxy.Source = url;
                         listProxy.Add(proxy);
                     });
                     VerifyAndSave(listProxy);

                 });
            }
            catch(Exception ex)
            {
                LogHelper.LogError("抓取小舒代理时出错：" + ex);
            }
            LogHelper.LogMsg("《《《小舒代理抓取完成");

        }
        /// <summary>
        /// 取代理页面的url
        /// </summary>
        private List<string> GetUrls()
        {
            var url = "http://www.xsdaili.com/";
            List<string> listUrl = new List<string>();
            try
            {
                var html = HttpHelper.DownloadHtml(url, null, TimeOut);

                if (string.IsNullOrEmpty(html))
                {
                    LogHelper.LogError("小舒代理，无法获取页面：" + url);
                    return listUrl;
                }
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                string titleWord;
                if (ConfigHelper.GetIsInit())
                {
                    //如果是第一次，抓取所有页面的代理，只抓取国内的，所以只取标题包含"国内"链接
                    titleWord = "国内";
                }
                else
                {
                    //如果不是第一次，只抓取当前国内的代理
                    titleWord = DateTime.Now.ToString("MM月dd日") + " 今日国内";
                }
                var totalPage = this.GetTotalPage(doc.DocumentNode);
                for(var i=1;i<=totalPage;i++)
                {
                    var pageUrl = url + "dayProxy/"+i+".html";
                    if(i!=1)
                    {
                        html = HttpHelper.DownloadHtml(pageUrl, null, TimeOut);
                        doc.LoadHtml(html);
                    }
                    var titleNodes = doc.DocumentNode.SelectNodes("//div[@class='col-md-12']/div/div[@class='title']/a");
                    foreach (var item in titleNodes)
                    {
                        var title = item.InnerText;
                        if (title.IndexOf(titleWord) == -1)
                        {
                            continue;
                        }
                        var attr = item.Attributes["href"];
                        if (attr != null && !string.IsNullOrWhiteSpace(attr.Value))
                        {
                            listUrl.Add(this.UrlCombin(url, attr.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return listUrl;
        }

        #region 取总页数
        /// <summary>
        /// 取总页数
        /// </summary>
        /// <param name="rootNode"></param>
        private int GetTotalPage(HtmlNode rootNode)
        {
            try
            {
                if (!ConfigHelper.GetIsInit())
                {
                    return 1;
                }
                var pageNodes = rootNode.SelectNodes("//div[@class='page']/a");
                if (pageNodes == null || pageNodes.Count <= 2)
                    return 1;
                var totalPage = ConvertHelper.ToInt(pageNodes[pageNodes.Count - 2].InnerText.Trim(),1);
                return totalPage;
            }
            catch(Exception ex)
            {
                return 1;
            }
        }
        #endregion
    }
}
