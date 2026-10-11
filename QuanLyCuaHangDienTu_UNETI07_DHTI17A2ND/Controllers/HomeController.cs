using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Models;
using System.Diagnostics;

namespace QuanLyCuaHangDienTu_UNETI07_DHTI17A2ND.Controllers
{
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
