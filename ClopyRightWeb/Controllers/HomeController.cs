using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClopyRightWeb.Models;

namespace ClopyRightWeb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new HomeViewModel();

            return View(model);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(HomeViewModel model)
        {
            var client = new HttpClient();

            var parameters = new NameValueCollection
            {
                {"reviewText", model.Keyword},
                {"minAppearance", model.MinAppearance.ToString()},
                {"minAroma", model.MinAroma.ToString()},
                {"minPalate", model.MinPalate.ToString()},
                {"minTaste", model.MinTaste.ToString()},
                {"minOverall", model.MinOverall.ToString()}
            };

            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = ConfigurationManager.AppSettings["APIHost"],
                Port = 8080,
                Path = "/api/Beer/",
                Query = ToQueryString(parameters)
            };

            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
            response.EnsureSuccessStatusCode();

            var recommendations = await response.Content.ReadAsAsync<IEnumerable<BeerRecommendation>>();

            model.BeerRecommendations = recommendations;

            return View(model);
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            return string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }
    }
}
