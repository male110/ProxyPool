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
                new Task(new CrawlXiaoShu().Start),
                new Task(new CrawlXiciDaili().Start),
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
                    //如果是第一次抓取，抓取完成后将配置文件改成false
                    if(ConfigHelper.GetIsInit())
                    {
                        ConfigHelper.ChangeToNotInit();
                    }
                    LastExecuteTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("抓取代理时出错：" + ex);
                }
            }
        }
        
    }
}
