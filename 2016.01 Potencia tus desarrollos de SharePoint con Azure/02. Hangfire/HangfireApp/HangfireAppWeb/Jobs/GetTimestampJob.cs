using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireAppWeb.Jobs
{
    public class GetTimestampJob
    {
        public void SaveTimestamp(Uri sharepointUrl)
        {
            using (var clientContext = AppOnlyContextHelper.GetAppOnlyClientContext(sharepointUrl))
            {
                var timestamp = DateTime.Now;

                var list = clientContext.Web.Lists.GetByTitle("SyncTimestamps");
                var item = list.AddItem(new Microsoft.SharePoint.Client.ListItemCreationInformation());
                item["Title"] = timestamp.ToString();
                item.Update();

                clientContext.ExecuteQuery();
            }
        }
    }
}
