using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.Linq;

namespace ProxyPool
{
	/// <summary>
	/// web.config操作类
    /// 饶丁辉
    /// 2016.10
	/// </summary>
	public static class ConfigHelper
	{
        private static IConfiguration cfg = null;
        static ConfigHelper()
        {
            cfg = new ConfigurationBuilder().Add(new JsonConfigurationSource { Path= "appsettings.json", ReloadOnChange=true}).Build();
        }
		/// <summary>
		/// 得到ConnectionString中的配置字符串信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public static string GetConnectionString()
		{
            return Get("connectString");
		}

		/// <summary>
		/// 得到AppSettings中的配置信息
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public static string Get(string key)
		{
            var section = cfg.GetSection(key);
            return section == null ? "" : section.Value;
		}
        /// <summary>
        /// 获取抓取代理的时间间隔，每隔多少分钟抓取一次
        /// </summary>
        /// <returns></returns>
        public static int GetCrawInterval()
        {
            var section = cfg.GetSection("CrawInterval");
            //如果没有配置返回37，默认37分钟抓取一次
            return section == null ? 37 : Convert.ToInt32(section.Value);
        }
        /// <summary>
        /// 每隔多久对已抓取的代理进行一次验证
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyInterval()
        {
            var section = cfg.GetSection("VerifyInterval");
            //如果没有配置返回13，默认13分钟验证一次
            return section == null ? 13 : Convert.ToInt32(section.Value);
        }
        /// <summary>
        /// 是否第一次抓取，如果是抓取所有的代理，如果不是，只抓取第一页的
        /// </summary>
        /// <returns></returns>
        public static bool GetIsInit()
        {
            var section = cfg.GetSection("IsInit");
            //如果没有配置返回false
            return section == null ? false : Convert.ToBoolean(section.Value);
        }
    }
}
