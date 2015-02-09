using MadPoint.BlogViewer.AppWeb.Extensions;
using MadPoint.BlogViewer.AppWeb.Models;
using MadPoint.BlogViewer.AppWeb.Resources;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MadPoint.BlogViewer.AppWeb.Services
{
    public class SpService
    {
        private readonly SharePointContext _spContext;

        public SpService(SharePointContext spContext)
        {
            _spContext = spContext;
        }

        public ViewerModel GetEntriesFrom(string blogUrl, int numberOfEntries, int bodyMaxLength)
        {
            var accessToken = _spContext.UserAccessTokenForSPHost;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception(Viewer.NotAccessTokenError);
            }

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(blogUrl, accessToken))
            {
                if (clientContext == null)
                {
                    throw new Exception(Viewer.BlogNotFoundError);
                }

                var web = clientContext.Web;
                var list = web.Lists.GetByTitle(Viewer.BlogListTitle);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                var viewXml = string.Format("<View><Query><OrderBy><FieldRef Name='PublishedDate' Ascending='False' /></OrderBy></Query><ViewFields><FieldRef Name='Title' /><FieldRef Name='Body' /><FieldRef Name='Author' /><FieldRef Name='PublishedDate' /></ViewFields><QueryOptions /><RowLimit>{0}</RowLimit></View>", numberOfEntries);
                var query = new CamlQuery { ViewXml = viewXml };

                var blogItems = list.GetItems(query);
                clientContext.Load(blogItems);
                clientContext.ExecuteQuery();

                var entries = new List<BlogEntryModel>();
                foreach (var item in blogItems)
                {
                    var authorUser = item["Author"] as FieldUserValue;
                    var htmlBody = item["Body"] as string;
                    var body = htmlBody.ToTextWithoutHtmlTags();

                    var firstPartOfBody = body.Length <= bodyMaxLength ? body : body.Substring(0, bodyMaxLength) + "...";
                    var publishedDate = (DateTime)item["PublishedDate"];
                    publishedDate = publishedDate.AddHours(1.0);
                    var entry = new BlogEntryModel
                    {
                        Id = item.Id,
                        Title = item["Title"] as string,
                        Content = firstPartOfBody,
                        Author = authorUser.LookupValue,
                        Date = publishedDate.ToString("HH:mm")
                    };
                    entries.Add(entry);
                }

                clientContext.Load(web, w => w.Title);
                clientContext.ExecuteQuery();
                var blogTitle = web.Title;

                var viewerModel = new ViewerModel
                {
                    BlogUrl = blogUrl,
                    BlogTitle = blogTitle,
                    BlogEntries = entries
                };
                return viewerModel;
            }
        }
    }
}