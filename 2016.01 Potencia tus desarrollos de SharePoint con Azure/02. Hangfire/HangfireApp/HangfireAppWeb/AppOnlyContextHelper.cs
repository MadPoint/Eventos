using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HangfireAppWeb
{
    public static class AppOnlyContextHelper
    {
        public static ClientContext GetAppOnlyClientContext(Uri sharepointUrl)
        {
            var appIdentity = "00000003-0000-0ff1-ce00-000000000000";
            var realm = TokenHelper.GetRealmFromTargetUrl(sharepointUrl);
            var accessToken = TokenHelper.GetAppOnlyAccessToken(appIdentity, sharepointUrl.Authority, realm);
            return TokenHelper.GetClientContextWithAccessToken(sharepointUrl.AbsoluteUri, accessToken.AccessToken);
        }
    }
}
