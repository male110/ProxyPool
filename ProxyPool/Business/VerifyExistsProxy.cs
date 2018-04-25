using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ProxyPool
{
    /// <summary>
    /// 定时验证已抓取的代理
    /// </summary>
    public class VerifyExistsProxy
    {
        DateTime LastVerifyTime { get; set; }
        public void Run()
        {
            while(true)
            {
                var s = DateTime.Now - LastVerifyTime;
                if (s.TotalMinutes < ConfigHelper.GetVerifyInterval())
                {
                    //每x分钟更新一次，如果间隔不足x分钟sleep
                    Thread.Sleep(1000 * 3);
                    continue;
                }
                LogHelper.LogMsg("》》》开始验证代理");
                try
                {
                    Verify();
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("验证代理时出错："+ex);
                }
                
                LastVerifyTime = DateTime.Now;
                LogHelper.LogMsg("《《《代理验证结束");
            }
        }

        public void Verify()
        {
            var pageSize = 100;
            var page = 1;
            int count = 0;
            var service = new ProxyService();
            do
            {
                try
                {
                    var lstProxy = service.GetList(page, pageSize);
                    count = lstProxy.Count;
                    var availableList = new List<Proxy>();
                    var deleteList = new List<Proxy>();
                    Parallel.ForEach(lstProxy, proxy =>
                    {
                        var state = ProxyVerify.IsAvailable(proxy);
                        if (state.IsAvailable)
                        {
                            //对于可用的，修改最后验证日期
                            proxy.Speed = state.Speed;
                            proxy.LastVerifyDate = DateTime.Now;
                            availableList.Add(proxy);
                        }
                        else
                        {
                            deleteList.Add(proxy);
                        }
                    });
                    //更新可用的
                    service.Update(availableList);
                    //删除不可用的
                    service.Delete(deleteList);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("验证代理时出错：" + ex);
                }

            } while (count >= pageSize);
        }
    }
}
