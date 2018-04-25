using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

namespace ProxyPool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = BuildWebHost(args);
                RunCrawlProxy();
                host.Run();
            }
            catch(Exception ex)
            {
                LogHelper.LogError("程序意外退出："+ex);
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        private static void RunCrawlProxy()
        {
            try
            {
                var crawlThread = new Thread(() => new PoolSpider().Run());
                crawlThread.Start();
                var verifyThread = new Thread(() => new VerifyExistsProxy().Run());
                verifyThread.Start();          
            }
            catch (Exception ex)
            {
                LogHelper.LogError("程序异常退出：" + ex);
            }
        }
    }
}
