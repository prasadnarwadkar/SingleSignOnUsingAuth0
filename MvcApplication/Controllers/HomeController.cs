using MvcApplication.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MvcApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return new ChallengeResult("Saml2", Url.Action("Index", "Home"));
            }
        }

        static string token { get; set; }

        static void setToken()
        {
            var client = new RestClient("https://icdaccessmanagement.who.int/connect/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "client_id=ee40acb4-9d20-4627-881f-5457b2b0c348_5b5ff832-8b30-4256-bc54-6c7dcaaaf829&client_secret=Q72LrS2l4MKNIA2BffpXEhfe1QzbPawV3S3VkjMf6R4=&grant_type=client_credentials&scope=icdapi_access", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JavaScriptSerializer ser = new JavaScriptSerializer();
            var deser = ser.Deserialize<AccessToken>(response.Content);

            token = deser.access_token;

        }

        static HomeController()
        {
            setToken();
        }

        [AllowAnonymous]
        public ActionResult Default3(string searchText = null)
        {
            var client = new RestClient();
            var request = new RestRequest();
            IRestResponse response = null;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                if (token?.Length > 0)
                {
                    client = new RestClient(string.Format("https://id.who.int/icd/entity/search?q={0}&flatResults=true&highlightingEnabled=true", searchText));
                    client.Timeout = -1;
                    request = new RestRequest(Method.GET);
                    request.AddHeader("API-Version", "v1");
                    request.AddHeader("Accept-Language", "en");
                    request.AddHeader("Authorization", "Bearer " + token);
                    response = client.Execute(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        setToken();

                        client = new RestClient(string.Format("https://id.who.int/icd/entity/search?q={0}&flatResults=true&highlightingEnabled=true", searchText));
                        client.Timeout = -1;
                        request = new RestRequest(Method.GET);
                        request.AddHeader("API-Version", "v1");
                        request.AddHeader("Accept-Language", "en");
                        request.AddHeader("Authorization", "Bearer " + token);
                        response = client.Execute(request);

                        JsonResult result = new JsonResult();
                        result.Data = JsonConvert.DeserializeObject<Root>(response.Content);
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        return result;
                    }
                    else
                    {
                        JsonResult result = new JsonResult();
                        result.Data = JsonConvert.DeserializeObject<Root>(response.Content);
                        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        return result;
                    }                    
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

    [AllowAnonymous]
        public ActionResult Default2(string searchText = null)
        {
            

            return View();
        }


    }
}