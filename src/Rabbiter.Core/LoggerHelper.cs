using Rabbiter.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbiter.Core
{
    public static class LoggerHelper
    {
        public static string Message(Message result, string message)
        {
            return $"{result.ConsumerTag}:{result.Exchange}-{result.DeliveryTag} {message}";
        }
    }
}
