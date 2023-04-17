using FPT_BOOKSTORE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FPT_BOOKSTORE.Utility.cs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers
{
    [Area(Constraintt.UnAuthenticatedArea)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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