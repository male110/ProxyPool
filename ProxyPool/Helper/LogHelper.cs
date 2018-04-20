using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;

namespace ProxyPool
{
    public class LogHelper
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 日志信息处理
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messages"></param>
        public static void LogMsg(string msg)
        {
            string message = string.Format("{0}\r\n", msg);
            logger.Info(message);
        }


        public static void LogError(Exception ex)
        {
            logger.Error(ex);
        }
        public static void LogError(string msg,Exception ex)
        {
            logger.Error( ex, msg);
        }
        public static void LogError(string msg)
        {
            logger.Error(msg);
        }
    }
}
