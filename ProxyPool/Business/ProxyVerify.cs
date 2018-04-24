using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProxyPool
{
     public  class ProxyVerify
    {
         public static VerifyState IsAvailable(Proxy proxy)
         {
             try
             {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                 WebProxy webproxy = new WebProxy(proxy.Adress, proxy.Port);
                 string html = HttpHelper.DownloadHtml("https://www.sogou.com/", webproxy,ConfigHelper.GetVerifyTimeOut());
                sw.Stop();
                 if (html.Contains("上网从搜狗开始"))
                 {
                    var byts = System.Text.Encoding.Default.GetBytes(html);
                    //计算用了多少秒
                    var time = sw.Elapsed.TotalMilliseconds / 1000;
                    //计算速度，单位字节
                    var speed = byts.Length / time;
                    return  new VerifyState() { IsAvailable = true, Speed = (int)speed };
                 }
             }
             catch(Exception ex)
             {
                Console.WriteLine(ex);
             }
            return new VerifyState() { IsAvailable = false, Speed = 0 };
         }
    }

    public class VerifyState
    {
        public bool IsAvailable { get; set; }
        public int Speed { get; set; }
    }
}
