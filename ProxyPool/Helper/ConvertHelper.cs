using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyPool
{
    public class ConvertHelper
    {
        /// <summary>
        /// 类型转换，字符型转int，如果转换失败返回默认值
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt(string str,int defaultValue)
        {
            int value = 0;
            if(!int.TryParse(str,out value))
            {
                value = defaultValue;
            }
            return value;
        }
        /// <summary>
        /// 字符型转bool,如果转换失败，返回默认值
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ToBoolean(string str,bool defaultValue)
        {
            bool value;
            if(!bool.TryParse(str,out value))
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
