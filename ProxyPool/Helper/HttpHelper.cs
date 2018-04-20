using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool
{
     public   class HttpHelper
    {
        /// <summary>
        /// 下载网页
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="proxy">代理</param>
        /// <param name="timeOut">超时时间，秒</param>
        /// <returns></returns>
        public static string DownloadHtml(string url,WebProxy proxy,int timeOut)
        {
            string source = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
        
                request.Proxy = proxy;

                request.Timeout = timeOut * 1000;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        if (response.ContentEncoding != null)
                        {
                            if (response.ContentEncoding.ToLower().Contains("gzip"))//解压
                            {
                                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                                {
                                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                    {
                                        source = reader.ReadToEnd();
                                    }
                                }
                            }
                            else if (response.ContentEncoding.ToLower().Contains("deflate"))//解压
                            {
                                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                                {
                                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                    {
                                        source = reader.ReadToEnd();
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (Stream stream = response.GetResponseStream())//原始
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {

                                    source = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                request.Abort();
            }
            catch(Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return source;
         
        }
    }
}
