using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbiter.IntegrationTests.Common
{
    public class Config
    {
        public string RabbitMqConnectionString { get; set; }
        public string RabbiterDbConnectionString { get; set; }
    }
}
