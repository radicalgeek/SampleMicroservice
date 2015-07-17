using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;



namespace Prototype.Infrastructure
{
    public class Configuration : IConfiguration
    {
        public string MessageQueue{
            get
            {
                return ConfigurationManager.AppSettings["RabbitMQConnectionString"];
                
            }
        }
    }
}
