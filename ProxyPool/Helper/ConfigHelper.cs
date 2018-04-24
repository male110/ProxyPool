using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.Linq;

namespace ProxyPool
{
	/// <summary>
	/// web.config������
    /// �Ķ���
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
		/// �õ�ConnectionString�е������ַ�����Ϣ
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public static string GetConnectionString()
		{
            return Get("connectString");
		}

		/// <summary>
		/// �õ�AppSettings�е�������Ϣ
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public static string Get(string key)
		{
            var section = cfg.GetSection(key);
            return section == null ? "" : section.Value;
		}
        /// <summary>
        /// ��ȡץȡ�����ʱ������ÿ�����ٷ���ץȡһ��
        /// </summary>
        /// <returns></returns>
        public static int GetCrawInterval()
        {
            var section = cfg.GetSection("CrawInterval");
            //���û�����÷���37��Ĭ��37����ץȡһ��
            return ConvertHelper.ToInt(section.Value, 37);
        }
        /// <summary>
        /// ÿ����ö���ץȡ�Ĵ������һ����֤
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyInterval()
        {
            var section = cfg.GetSection("VerifyInterval");
            //���û�����÷���13��Ĭ��13������֤һ��
            return ConvertHelper.ToInt(section.Value, 13);
        }
        /// <summary>
        /// �Ƿ��һ��ץȡ�������ץȡ���еĴ���������ǣ�ֻץȡ��һҳ��
        /// </summary>
        /// <returns></returns>
        public static bool GetIsInit()
        {
            var section = cfg.GetSection("IsInit");
            //���û�����÷���false
            return ConvertHelper.ToBoolean(section.Value, false);
        }

        /// <summary>
        /// ץȡ����ʱ����ҳ���صĳ�ʱʱ�䣬��λ��
        /// </summary>
        /// <returns></returns>
        public static int GetCrawlTimeOut()
        {
            var section = cfg.GetSection("CrawlTimeOut");
            //Ĭ�ϳ�ʱʱ��5��
            return ConvertHelper.ToInt(section.Value, 5);
        }
        /// <summary>
        /// ��֤����ʱ�ĳ�ʱʱ�䣬��λ��
        /// </summary>
        /// <returns></returns>
        public static int GetVerifyTimeOut()
        {
            var section = cfg.GetSection("VerifyTimeOut");
            //Ĭ�ϳ�ʱʱ��3��
            return ConvertHelper.ToInt(section.Value, 3);
        }
    }
}
