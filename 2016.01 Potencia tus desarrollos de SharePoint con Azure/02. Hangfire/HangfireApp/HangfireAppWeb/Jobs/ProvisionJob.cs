using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireAppWeb.Jobs
{
    public class ProvisionJob
    {
        public void Provision(Uri sharepointUrl)
        {
            using (var clientContext = AppOnlyContextHelper.GetAppOnlyClientContext(sharepointUrl))
            {
                var list = clientContext.Web.Lists.Add(new ListCreationInformation
                {
                    Title = "SyncTimestamps",
                    TemplateType = (int)ListTemplateType.GenericList
                });

                Thread.Sleep(TimeSpan.FromSeconds(20));

                clientContext.Load(list);
                clientContext.ExecuteQuery();
            }
        }
    }
}
