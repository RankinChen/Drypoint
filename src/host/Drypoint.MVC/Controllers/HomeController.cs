using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Drypoint.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using IdentityModel.Client;

namespace Drypoint.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            var user = User.Identity;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> TestAPI()
        {
            var client = _clientFactory.CreateClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44333");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            client.SetBearerToken(accessToken);


            var response = await client.GetAsync("https://localhost:44332/api/Demo");
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new Exception(response.ReasonPhrase);
            //}

            var content = await response.Content.ReadAsStringAsync();

            return View("TestAPI", content);
        }

        [Authorize(Policy = "SmithInSomewhere")]
        public IActionResult Privacy()
        {
            var accessToken = HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken).Result;
            var idToken = HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken).Result;

            var refreshToken = HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken).Result;

            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
