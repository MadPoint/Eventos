using System.Web;
using System.Web.Mvc;

namespace Beezy.AzureML.Proxy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
