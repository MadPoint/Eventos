using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beezy.AzureML.Proxy.Models
{
    public class RequestInfo
    {
        public string ApiKey
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }
    }
}
