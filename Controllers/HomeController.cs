using Microsoft.AspNetCore.Mvc;

namespace Turma_5413_TP_BrunoSilva.Controllers
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

        public IActionResult Welcome()
        {
            return View();
        }
    }
}
