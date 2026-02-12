using System.Diagnostics;
using Cinemalek.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
