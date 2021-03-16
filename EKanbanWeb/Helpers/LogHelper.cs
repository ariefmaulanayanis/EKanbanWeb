using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Helpers
{
    public class LogHelper
    {
        private readonly ILogger _logger;
        private string _message;

        public LogHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void WriteErrorLog(Exception e, string message)
        {
            if (message != "") _message += "\n";
            else _message = "";
            WriteErrorLog(e);
        }

        public void WriteErrorLog(Exception e)
        {
            _logger.LogError(
                _message + "Message: " + e.Message +
                "\nData: " + e.Data +
                "\nSource: " + e.Source +
                "\nStackTrace: " + e.StackTrace +
                "\nInnerException: " + e.InnerException
                );
        }
        public void WriteWarningLog(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
