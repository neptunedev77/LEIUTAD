using Microsoft.AspNetCore.Mvc;

namespace LEIUTAD.Controllers
{
    public class BibliotecaController : Controller
    {
        [HttpGet]
        public IActionResult Informacoes()
        {
            return View();
        }
    }
}
