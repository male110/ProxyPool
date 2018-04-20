using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool
{
    /// <summary>
    /// 定时验证已抓取的代理
    /// </summary>
    public class VerifyExistsProxy
    {
        DateTime LastVerifyTime { get; set; }
        public void Start()
        {
            var verifyTask = new Task(Verify);
            while(true)
            {
                try
                {
                    verifyTask.Start();
                    verifyTask.Wait();
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                }
                
                LastVerifyTime = DateTime.Now;
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
                var lstProxy=service.GetList(page,pageSize);
                var availableList = new List<Proxy>();
                var deleteList = new List<Proxy>();
                Parallel.ForEach(lstProxy,proxy=> {
                    var state=ProxyVerify.IsAvailable(proxy);
                    if(state.IsAvailable)
                    {
                        proxy.Speed = state.Speed;
                        proxy.LastVerifyDate = DateTime.Now;
                        availableList.Add(proxy);
                    }
                    else
                    {
                        deleteList.Add(proxy);
                    }
                });

            } while (count>=pageSize);
        }
    }
}
