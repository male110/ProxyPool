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
        private int timeOut = 5;//下载页面的超时时间
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取代理页面的url
        /// </summary>
        public List<string> GetUrls()
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
                        html = HttpHelper.DownloadHtml(url, null, TimeOut);
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
            if(!ConfigHelper.GetIsInit())
            {
                return 1;
            }
            var pageNodes=rootNode.SelectNodes("//div[@class='page']/a");
            if (pageNodes == null || pageNodes.Count <= 2)
                return 1;
            var totalPage = Convert.ToInt32(pageNodes[pageNodes.Count - 2].InnerText.Trim());
            return totalPage;
        }
        #endregion
    }
}
