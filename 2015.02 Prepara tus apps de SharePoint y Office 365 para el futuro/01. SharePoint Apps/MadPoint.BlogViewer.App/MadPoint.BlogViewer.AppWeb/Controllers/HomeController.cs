using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MadPoint.BlogViewer.AppWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            SetCurrentSpCulture();
            return View();
        }

        private void SetCurrentSpCulture()
        {
            var spLanguage = Request.QueryString["SPLanguage"];
            if (spLanguage != null)
            {
                var language = spLanguage;
                var culture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        } 
    }
}