using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace BomberSoz.Core
{
    //TODO: Реализовать
    public static class Logging
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void WriteInfo(string message)
        {
            _logger.Info(message);
        }
        public static void WriteDebbug(string message)
        {
            _logger.Debug(message);
        }
        public static void WriteTrace(string message)
        {
            _logger.Trace(message);
        }
    }
}

