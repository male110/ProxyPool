using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProxyPool
{
    public class ProxyService
    {
        private ProxyDBContext dbcontext = new ProxyDBContext();
        #region 取数据
        #region 随机取一个代理
        /// <summary>
        /// 随机取一个代理
        /// </summary>
        /// <returns></returns>
        public Proxy GetProxy()
        {
            int total = GetCount();
            var rand = new Random();
            var skip = rand.Next(0, total);
            try
            {
                if (skip == 0)
                {
                    return dbcontext.Proxy.First();
                }
                else
                {
                    return dbcontext.Proxy.Skip(skip).First();
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        #endregion

        #region 分页取代理
        /// <summary>
        /// 分页取代理
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Proxy> GetList(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var totalPage = (int)Math.Ceiling((decimal)GetCount() / pageSize);

            var ret = new List<Proxy>();
            try
            {
                return dbcontext.Proxy.Skip((page-1) * pageSize).Take(pageSize).OrderBy(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
            return ret;
        }
        #endregion

        #region 取记录数
        /// <summary>
        /// 取记录数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return dbcontext.Proxy.Count();
        }
        #endregion
        #endregion

        #region 删除
        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="p"></param>
        public void DeleteProxy(Proxy p)
        {
            if (p == null)
                return;
            try
            {
                dbcontext.Proxy.Remove(p);
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("删除代理失败：" + ex);
            }
        }
        public int Delete(List<Proxy> lstProxy)
        {
            if (lstProxy == null|| lstProxy.Count == 0)
                return 0;
            foreach(var item in lstProxy)
            {
                dbcontext.Proxy.Remove(item);
            }
            return dbcontext.SaveChanges();
        }
        #endregion

        #region 新增代理
        /// <summary>
        /// 新增代理
        /// </summary>
        /// <param name="proxy"></param>
        public void Add(Proxy proxy)
        {
            try
            {
                //判断是否已存在
                Proxy existItem=null;
                try
                {
                    existItem = dbcontext.Proxy.Where(p => p.Adress == proxy.Adress && p.Port == proxy.Port).FirstOrDefault();
                }
                catch { }

                if (existItem != null)
                    return;
                proxy.CreateDate = DateTime.Now;
                dbcontext.Proxy.Add(proxy);
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("新增代理失败：" + ex);
            }
        }
        /// <summary>
        /// 批量新增代理
        /// </summary>
        /// <param name="lstProxy"></param>
        public void Add(List<Proxy> lstProxy)
        {
            if (lstProxy == null || lstProxy.Count == 0)
                return;
            try
            {
                var newList = FilterExist(lstProxy);
                foreach (var p in newList)
                {
                    p.CreateDate = DateTime.Now;
                    dbcontext.Proxy.Add(p);
                }
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }

        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改代理
        /// </summary>
        /// <param name="item"></param>
        public void Update(Proxy item)
        {
            dbcontext.Proxy.Update(item);
            dbcontext.SaveChanges();
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="proxyList"></param>
        public void Update(List<Proxy> proxyList)
        {
            if (proxyList == null || proxyList.Count == 0)
                return;
            try
            {
                foreach (var item in proxyList)
                {
                    dbcontext.Update(item);
                }
                dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("批量修改代理时出错：" + ex);
            }
        }
        #endregion

        #region 过滤已存在的
        /// <summary>
        /// 过滤掉已存在的
        /// </summary>
        public List<Proxy> FilterExist(List<Proxy> lstProxy)
        {
            if (lstProxy != null || lstProxy.Count == 0)
                return lstProxy;
            var arrAdress = lstProxy.Select(x => x.Adress).ToList();
            var existList = dbcontext.Proxy.Where(p => arrAdress.Contains(p.Adress)).ToList();
            var newList = lstProxy.Where(p => !existList.Exists(e => e.Adress == p.Adress && e.Port == p.Port)).ToList();
            return newList;
        }
        #endregion


    }
}
