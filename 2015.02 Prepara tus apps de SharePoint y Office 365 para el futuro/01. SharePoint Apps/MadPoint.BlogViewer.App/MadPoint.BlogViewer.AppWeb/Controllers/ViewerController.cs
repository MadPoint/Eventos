using MadPoint.BlogViewer.AppWeb.Models;
using MadPoint.BlogViewer.AppWeb.Services;
using MadPoint.BlogViewer.AppWeb.Resources;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace MadPoint.BlogViewer.AppWeb.Controllers
{
    public class ViewerController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            SetCurrentSpCulture();

            ErrorModel errorModel;
            var blogUrl = Request.QueryString["blogUrl"];
            if (string.IsNullOrWhiteSpace(blogUrl) || blogUrl.Equals(Viewer.BlogUrlDefaultValue))
            {
                errorModel = new ErrorModel { ErrorMessage = Viewer.BlogUrlError };
                return View("Error", errorModel);
            }
            else
            {
                const string Layouts15Text = "_layouts/15";
                if (blogUrl.Contains(Layouts15Text))
                {
                    var indexOfLayouts15 = blogUrl.IndexOf(Layouts15Text);
                    blogUrl = blogUrl.Substring(0, indexOfLayouts15);
                }
            }

            var numberOfEntries = int.Parse(Request.QueryString["numberOfEntries"]);
            if (numberOfEntries < 0)
            {
                errorModel = new ErrorModel { ErrorMessage = Viewer.NumberOfEntriesError };
                return View("Error", errorModel);
            }

            var bodyMaxLength = int.Parse(Request.QueryString["entriesBodyMaxLength"]);
            if (bodyMaxLength < 0)
            {
                errorModel = new ErrorModel { ErrorMessage = Viewer.EntriesBodyMaxLengthError };
                return View("Error", errorModel);
            }

            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                var spService = new SpService(spContext);
                var viewerModel = spService.GetEntriesFrom(blogUrl, numberOfEntries, bodyMaxLength);
                return View(viewerModel);
            }
            catch (ServerUnauthorizedAccessException)
            {
                errorModel = new ErrorModel { ErrorMessage = Viewer.UnauthorizedError };
            }
            catch (ServerException)
            {
                var errorMessage = string.Format(Viewer.NotBlogError, Viewer.BlogListTitle);
                errorModel = new ErrorModel { ErrorMessage = errorMessage };
            }
            catch (Exception exception)
            {
                errorModel = new ErrorModel { ErrorMessage = exception.Message };
            }

            return View("Error", errorModel);
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
