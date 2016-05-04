using Beezy.AzureML.Proxy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Beezy.AzureML.Proxy.Controllers
{
    public class ProxyController : ApiController
    {
        // POST api/values
        public HttpResponseMessage Post([FromBody]RequestInfo info)
        {
            var client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Authorization", "Bearer " + info.ApiKey);

            try
            {
                var response = client.UploadString(info.Url, info.Data);
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            catch(WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                return Request.CreateErrorResponse(response.StatusCode, ex.Message);
            }
        }
    }
}
