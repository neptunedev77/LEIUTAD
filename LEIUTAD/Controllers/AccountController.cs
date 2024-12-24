using LEIUTAD.Data;
using LEIUTAD.Models;
using LEIUTAD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace LEIUTAD.Controllers
{
    public class AccountController : Controller
    {
        private readonly LEIUTADContext _context;

        public AccountController(LEIUTADContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Criar o novo leitor
                var leitor = new Leitor
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    Tele_n = model.Tele_n,
                    Data_n = model.Data_n,
                    Endereco = model.Endereco,
                    Endereco_n = model.Endereco_n,
                    Localidade = model.Localidade,
                    Pais = model.Pais,
                    Estado = false // Conta ainda não verificada
                };

                // Gerar PasswordHash e PasswordSalt
                using (var hmac = new HMACSHA512())
                {
                    leitor.PasswordSalt = hmac.Key;
                    leitor.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                }

                // Adicionar o leitor à base de dados
                _context.Leitor.Add(leitor);
                _context.SaveChanges();

                // Redirecionar após o registo
                return RedirectToAction("Index", "Livros");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Leitor.SingleOrDefault(u => u.Email == model.Email);

                // Verificar se o utilizador existe
                if (user == null)
                {
                    ModelState.AddModelError("", "Utilizador ou password inválidos.");
                    return View(model);
                }

                // Verificar password
                using (var hmac = new HMACSHA512(user.PasswordSalt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                    if (!computedHash.SequenceEqual(user.PasswordHash))
                    {
                        ModelState.AddModelError("", "Utilizador ou password inválidos.");
                        return View(model);
                    }
                }

                // Verificar o estado (email verificado)
                if (!user.Estado)
                {
                    ModelState.AddModelError("", "A conta ainda não foi verificada.");
                    return View(model);
                }

                // Guardar informações de sessão
                HttpContext.Session.SetString("UserName", user.Nome);
                HttpContext.Session.SetInt32("UserId", user.ID_user);

                return RedirectToAction("Index", "Livros");
            }

            return View(model);
        }

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
