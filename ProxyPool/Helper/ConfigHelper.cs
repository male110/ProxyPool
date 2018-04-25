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
	/// web.config������
    /// �Ķ���
    /// 2016.10
	/// </summary>
	public static class ConfigHelper
	{
        private static IConfiguration cfg = null;
        public static void SetConfiguration(IConfiguration configuration)
        {
            cfg = configuration;
        }

        #region ȡ�����ַ���
        /// <summary>
        /// �õ�ConnectionString�е������ַ�����Ϣ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionString()
		{
            return Get("connectString");
		}
        #endregion

        #region ������ȡ������Ϣ
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
        #endregion

        #region ��ȡץȡ�����ʱ����
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
        #endregion

        #region ��֤����ļ��ʱ��
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
        #endregion

        #region �Ƿ��һ��ץȡ
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
        #endregion

        #region ץȡ����ĳ�ʱʱ��
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
        #endregion

        #region ��֤����ĳ�ʱʱ��
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
        #endregion

        /// <summary>
        /// �޸������ģ����Ƿ��һ��ץȡ�ĳ�fals
        /// </summary>
        public static void ChangeToNotInit()
        {
            //�����Ƿ��Ѿ���false
            if(!GetIsInit())
            {
                return;
            }
            string strName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            string strNewJson = "";//�޸ĺ��json
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
