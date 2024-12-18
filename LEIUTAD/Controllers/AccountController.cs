using LEIUTAD.Data;
using Microsoft.AspNetCore.Mvc;

namespace LEIUTAD.Controllers
{
    public class AccountController : Controller
    {
        private readonly LEIUTADContext _context;

        public AccountController(LEIUTADContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register() {

            return View();
        }
    }
}
