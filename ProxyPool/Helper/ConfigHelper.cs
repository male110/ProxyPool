using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
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
        public static void SetConfiguration(IConfiguration configuration)
        {
            cfg = configuration;
        }

        #region 取连接字符串
        /// <summary>
        /// 得到ConnectionString中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionString()
		{
            return Get("connectString");
		}
        #endregion

        #region 有名称取配置信息
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
        #endregion

        #region 获取抓取代理的时间间隔
        /// <summary>
        /// 获取抓取代理的时间间隔，每隔多少分钟抓取一次
        /// </summary>
        /// <returns></returns>
        public static int GetCrawInterval()
        {
            var section = cfg.GetSection("CrawInterval");
            //如果没有配置返回37，默认37分钟抓取一次
            return ConvertHelper.ToInt(section.Value, 37);
        }
        #endregion

        #region 验证代理的间隔时间
        /// <summary>
        /// 每隔多久对已抓取的代理进行一次验证
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyInterval()
        {
            var section = cfg.GetSection("VerifyInterval");
            //如果没有配置返回13，默认13分钟验证一次
            return ConvertHelper.ToInt(section.Value, 13);
        }
        #endregion

        #region 是否第一次抓取
        /// <summary>
        /// 是否第一次抓取，如果是抓取所有的代理，如果不是，只抓取第一页的
        /// </summary>
        /// <returns></returns>
        public static bool GetIsInit()
        {
            var section = cfg.GetSection("IsInit");
            //如果没有配置返回false
            return ConvertHelper.ToBoolean(section.Value, false);
        }
        #endregion

        #region 抓取代理的超时时间
        /// <summary>
        /// 抓取代理时，网页下载的超时时间，单位秒
        /// </summary>
        /// <returns></returns>
        public static int GetCrawlTimeOut()
        {
            var section = cfg.GetSection("CrawlTimeOut");
            //默认超时时间5秒
            return ConvertHelper.ToInt(section.Value, 5);
        }
        #endregion

        #region 验证代理的超时时间
        /// <summary>
        /// 验证代理时的超时时间，单位秒
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyTimeOut()
        {
            var section = cfg.GetSection("VerifyTimeOut");
            //默认超时时间3秒
            return ConvertHelper.ToInt(section.Value, 3);
        }
        #endregion

        /// <summary>
        /// 修改配置文，把是否第一次抓取改成fals
        /// </summary>
        public static void ChangeToNotInit()
        {
            //先判是否已经是false
            if(!GetIsInit())
            {
                return;
            }
            string strName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            string strNewJson = "";//修改后的json
            using (StreamReader reader =new StreamReader(strName))
            {
                string json =reader.ReadToEnd();
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["IsInit"] = false;
                strNewJson = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            }
            using (StreamWriter writer = new StreamWriter(strName))
            {
                writer.Write(strNewJson);
            }
        }
    }
}
