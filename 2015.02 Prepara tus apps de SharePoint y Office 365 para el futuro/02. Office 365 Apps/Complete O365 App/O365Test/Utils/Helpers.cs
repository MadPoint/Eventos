using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace O365Test.Utils
{
    public static class Helpers
    {
        public static void SaveInCache(string name, object value)
        {
            System.Web.HttpContext.Current.Session[name] = value;
        }
        public static object GetFromCache(string name)
        {
            return System.Web.HttpContext.Current.Session[name];
        }
        public static void RemoveFromCache(string name)
        {
            System.Web.HttpContext.Current.Session.Remove(name);
        }
    }
}