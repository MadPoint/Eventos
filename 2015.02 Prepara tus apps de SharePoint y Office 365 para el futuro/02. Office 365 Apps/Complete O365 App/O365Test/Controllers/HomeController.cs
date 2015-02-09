using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OutlookServices;
using Microsoft.Office365.SharePoint.CoreServices;
using O365Test.Models;
using O365Test.Utils;

namespace O365Test.Controllers
{
    public class HomeController : Controller
    {
        private const string spSite = "https://devcamp2014.sharepoint.com";
        private const string discoResource = "https://api.office.com/discovery/";
        private const string discoEndpoint = "https://api.office.com/discovery/v1.0/me/";
        public async Task<ActionResult> Index(string code)
        {
            AuthenticationContext authContext = new AuthenticationContext(
               ConfigurationManager.AppSettings["ida:AuthorizationUri"] + "/common",
               true);

            ClientCredential creds = new ClientCredential(
                ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);

            DiscoveryClient disco = Helpers.GetFromCache("DiscoveryClient") as DiscoveryClient;

            //Redirect to login page if we do not have an 
            //authorization code for the Discovery service
            if (disco == null && code == null)
            {
                Uri redirectUri = authContext.GetAuthorizationRequestURL(
                    discoResource,
                    creds.ClientId,
                    new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                    UserIdentifier.AnyUser,
                    string.Empty);

                return Redirect(redirectUri.ToString());
            }

            //Create a DiscoveryClient using the authorization code
            if (disco == null && code != null)
            {

                disco = new DiscoveryClient(new Uri(discoEndpoint), async () =>
                {

                    var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                        code,
                        new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                        creds);

                    return authResult.AccessToken;
                });

            }

            //Discover required capabilities
            CapabilityDiscoveryResult contactsDisco = await disco.DiscoverCapabilityAsync("Contacts");
            CapabilityDiscoveryResult filesDisco = await disco.DiscoverCapabilityAsync("MyFiles");

            Helpers.SaveInCache("ContactsDiscoveryResult", contactsDisco);
            Helpers.SaveInCache("FilesDiscoveryResult", filesDisco);

            List<MyDiscovery> discoveries = new List<MyDiscovery>(){
                new MyDiscovery(){
                    Capability = "Contacts",
                    EndpointUri = contactsDisco.ServiceEndpointUri.OriginalString,
                    ResourceId = contactsDisco.ServiceResourceId,
                    Version = contactsDisco.ServiceApiVersion
                },
                new MyDiscovery(){
                    Capability = "My Files",
                    EndpointUri = filesDisco.ServiceEndpointUri.OriginalString,
                    ResourceId = filesDisco.ServiceResourceId,
                    Version = filesDisco.ServiceApiVersion
                }
            };

            return View(discoveries);

        }

        public async Task<ActionResult> Contacts(string code)
        {
            AuthenticationContext authContext = new AuthenticationContext(
               ConfigurationManager.AppSettings["ida:AuthorizationUri"] + "/common",
               true);

            ClientCredential creds = new ClientCredential(
                ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);

            //Get the discovery information that was saved earlier
            CapabilityDiscoveryResult cdr = Helpers.GetFromCache("ContactsDiscoveryResult") as CapabilityDiscoveryResult;

            //Get a client, if this page was already visited
            OutlookServicesClient outlookClient = Helpers.GetFromCache("OutlookClient") as OutlookServicesClient;

            //Get an authorization code if needed
            if (outlookClient == null && cdr != null && code == null)
            {
                Uri redirectUri = authContext.GetAuthorizationRequestURL(
                    cdr.ServiceResourceId,
                    creds.ClientId,
                    new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                    UserIdentifier.AnyUser,
                    string.Empty);

                return Redirect(redirectUri.ToString());
            }

            //Create the OutlookServicesClient
            if (outlookClient == null && cdr != null && code != null)
            {

                outlookClient = new OutlookServicesClient(cdr.ServiceEndpointUri, async () =>
                {

                    var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                        code,
                        new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                        creds);

                    return authResult.AccessToken;
                });

                Helpers.SaveInCache("OutlookClient", outlookClient);
            }

            //Get the contacts
            var contactsResults = await outlookClient.Me.Contacts.ExecuteAsync();
            List<MyContact> contactList = new List<MyContact>();

            foreach (var contact in contactsResults.CurrentPage.OrderBy(c => c.Surname))
            {
                contactList.Add(new MyContact
                {
                    Id = contact.Id,
                    GivenName = contact.GivenName,
                    Surname = contact.Surname,
                    DisplayName = contact.Surname + ", " + contact.GivenName,
                    CompanyName = contact.CompanyName,
                    EmailAddress1 = contact.EmailAddresses.FirstOrDefault().Address,
                    BusinessPhone1 = contact.BusinessPhones.FirstOrDefault(),
                    HomePhone1 = contact.HomePhones.FirstOrDefault()
                });
            }

            //Save the contacts
            Helpers.SaveInCache("ContactList", contactList);

            //Show the contacts
            return View(contactList);

        }

        public async Task<ActionResult> Files(string code)
        {
            AuthenticationContext authContext = new AuthenticationContext(
               ConfigurationManager.AppSettings["ida:AuthorizationUri"] + "/common",
               true);

            ClientCredential creds = new ClientCredential(
                ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);

            //Get the discovery information that was saved earlier
            CapabilityDiscoveryResult cdr = Helpers.GetFromCache("FilesDiscoveryResult") as CapabilityDiscoveryResult;

            //Get a client, if this page was already visited
            SharePointClient sharepointClient = Helpers.GetFromCache("SharePointClient") as SharePointClient;

            //Get an authorization code, if needed
            if (sharepointClient == null && cdr != null && code == null)
            {
                Uri redirectUri = authContext.GetAuthorizationRequestURL(
                    cdr.ServiceResourceId,
                    creds.ClientId,
                    new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                    UserIdentifier.AnyUser,
                    string.Empty);

                return Redirect(redirectUri.ToString());
            }

            //Create the SharePointClient
            if (sharepointClient == null && cdr != null && code != null)
            {

                sharepointClient = new SharePointClient(cdr.ServiceEndpointUri, async () =>
                {

                    var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                        code,
                        new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                        creds);

                    return authResult.AccessToken;
                });

                Helpers.SaveInCache("SharePointClient", sharepointClient);
            }

            //Get the files
            var filesResults = await sharepointClient.Files.ExecuteAsync();

            var fileList = new List<MyFile>();

            foreach (var file in filesResults.CurrentPage.Where(f => f.Name != "Shared with Everyone").OrderBy(e => e.Name))
            {
                fileList.Add(new MyFile
                {
                    Id = file.Id,
                    Name = file.Name,
                    Url = file.WebUrl
                });
            }

            //Save the files
            Helpers.SaveInCache("FileList", fileList);

            //Show the files
            return View(fileList);

        }

        public async Task<ActionResult> Projects(ViewModel submitModel, string code)
        {
            //If the New Project form needs to be displayed
            if (submitModel.Project == null && code == null)
            {
                ViewModel formModel = new ViewModel();
                formModel.Contacts = Helpers.GetFromCache("ContactList") as List<MyContact>;
                formModel.Files = Helpers.GetFromCache("FileList") as List<MyFile>;
                return View(formModel);
            }
            // A new project was submitted
            else
            {
                if (submitModel.Project != null)
                {
                    Helpers.SaveInCache("SubmitModel", submitModel);
                }

                AuthenticationContext authContext = new AuthenticationContext(
                   ConfigurationManager.AppSettings["ida:AuthorizationUri"] + "/common",
                   true);

                ClientCredential creds = new ClientCredential(
                    ConfigurationManager.AppSettings["ida:ClientID"],
                    ConfigurationManager.AppSettings["ida:Password"]);

                //Get an authorization code, if necessary
                if (code == null)
                {
                    Uri redirectUri = authContext.GetAuthorizationRequestURL(
                        spSite,
                        creds.ClientId,
                        new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                        UserIdentifier.AnyUser,
                        string.Empty);

                    return Redirect(redirectUri.ToString());
                }
                else
                {
                    //Get the access token
                    var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                        code,
                        new Uri(Request.Url.AbsoluteUri.Split('?')[0]),
                        creds);

                    string accessToken = authResult.AccessToken;

                    //Build SharePoint RESTful API endpoint for the list items
                    StringBuilder requestUri = new StringBuilder()
                      .Append(spSite)
                      .Append("/_api/web/lists/getbyTitle('Research Projects')/items");

                    //Create an XML message with the new project data
                    //This message will be POSTED to the SharePoint API endpoint
                    XNamespace atom = "http://www.w3.org/2005/Atom";
                    XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
                    XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

                    submitModel = Helpers.GetFromCache("SubmitModel") as ViewModel;
                    string description = (Helpers.GetFromCache("FileList") as List<MyFile>).Where(f => f.Url == submitModel.Project.DocumentLink).First().Name;
                    string url = submitModel.Project.DocumentLink;
                    string title = submitModel.Project.Title;
                    string owner = submitModel.Project.Owner;

                    XElement message = new XElement(atom + "entry",
                        new XAttribute(XNamespace.Xmlns + "d", d),
                        new XAttribute(XNamespace.Xmlns + "m", m),
                        new XElement(atom + "category", new XAttribute("term", "SP.Data.Research_x0020_ProjectsListItem"), new XAttribute("scheme", "http://schemas.microsoft.com/ado/2007/08/dataservices/scheme")),
                        new XElement(atom + "content", new XAttribute("type", "application/xml"),
                            new XElement(m + "properties",
                                new XElement(d + "Statement", new XAttribute(m + "type", "SP.FieldUrlValue"),
                                    new XElement(d + "Description", description),
                                    new XElement(d + "Url", url)),
                                new XElement(d + "Title", title),
                                new XElement(d + "Owner", owner))));

                    StringContent requestData = new StringContent(message.ToString());

                    //POST the data to the endpoint
                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri.ToString());
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    requestData.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/atom+xml");
                    request.Content = requestData;
                    HttpResponseMessage response = await client.SendAsync(request);

                    //Show the Finished screen
                    return RedirectToAction("Finished");
                }
            }


        }

        public ActionResult Finished()
        {
            return View();
        }
    }
}