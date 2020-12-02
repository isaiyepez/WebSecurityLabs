using AcmeLib;
using AcmeWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AcmeWebsite.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment env;
        public HomeController(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public IActionResult Index()
        {
            ViewBag.Message = "";
            return View();
        }        

        /// <summary>
        /// Action for db reset link.  Executes the database initial script.
        /// </summary>
        /// <returns>Success page</returns>
        public IActionResult DbReset()
        {
            var filename = System.IO.Path.Combine(env.WebRootPath, @"DbReset.sql");
            BankService.DbReset(filename);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
