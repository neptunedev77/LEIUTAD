using LEIUTAD.Data;
using LEIUTAD.Models;
using LEIUTAD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using LEIUTAD.Services;


namespace LEIUTAD.Controllers
{
    public class AccountController : Controller
    {
        private readonly LEIUTADContext _context;
        private readonly EmailService _emailService;

        public AccountController(LEIUTADContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }



        [HttpGet]
        public IActionResult Register()
        {
            // Lista de países
         ViewBag.Paises = new List<string> {
        "Portugal", "Brasil", "Espanha", "França", "Alemanha",
        "Reino Unido", "Itália", "Estados Unidos", "Canadá", "Austrália",
        "Japão", "China", "Rússia", "Índia", "Argentina", "México",
        "Chile", "Uruguai", "África do Sul", "Angola", "Moçambique"
        //
    };

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
                return RedirectToAction("Login", "Account");
            }

            // Caso a validação falhe, recarregar a lista de países para a View
            ViewBag.Paises = new List<string>
            {
                "Portugal", "Brasil", "Espanha", "França", "Alemanha",
                "Reino Unido", "Itália", "Estados Unidos", "Canadá", "Austrália",
                "Japão", "China", "Rússia", "Índia", "Argentina", "México",
                "Chile", "Uruguai", "África do Sul", "Angola", "Moçambique"
            };

            return View(model);
        }

        public void EnviarEmailVerificacao(Leitor leitor)
        {
            var token = Guid.NewGuid().ToString(); // Gera um token único
            leitor.TokenVerificacao = token;
            _context.SaveChanges();

            var urlVerificacao = Url.Action("VerificarConta", "Account", new { token }, Request.Scheme);
            var subject = "Verificação de Conta";
            var body = $"<p>Clique no link abaixo para verificar a sua conta:</p><p><a href='{urlVerificacao}'>Verificar Conta</a></p>";

            _emailService.SendEmail(leitor.Email, subject, body);
        }


        [HttpGet]
        public IActionResult VerificarConta(string token)
        {
            var leitor = _context.Leitor.FirstOrDefault(l => l.TokenVerificacao == token);
            if (leitor == null)
            {
                return NotFound("Token inválido.");
            }

            leitor.Estado = true; // Verifica a conta
            leitor.TokenVerificacao = null; // Remove o token
            _context.SaveChanges();

            return View("ContaVerificada"); // Mostra uma página de confirmação
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
                // Verificar na tabela Leitor
                var user = _context.Leitor.SingleOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    using (var hmac = new HMACSHA512(user.PasswordSalt))
                    {
                        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                        if (!computedHash.SequenceEqual(user.PasswordHash))
                        {
                            ModelState.AddModelError("", "Utilizador ou password inválidos.");
                            return View(model);
                        }
                    }

                    if (user.IsBloqueado)
                    {
                        ModelState.AddModelError("", "A sua conta está bloqueada. Contacte o suporte para mais informações.");
                        return View(model);
                    }

                    if (!user.Estado)
                    {
                        ModelState.AddModelError("", "A conta ainda não foi verificada.");
                        return View(model);
                    }

                    HttpContext.Session.SetString("UserName", user.Nome);
                    HttpContext.Session.SetInt32("UserId", user.ID_user);
                    HttpContext.Session.SetString("UserRole", "Leitor");

                    return RedirectToAction("Index", "Livros");
                }

                // Verificar na tabela Bibliotecarios
                var bibliotecario = _context.Bibliotecarios.SingleOrDefault(b => b.Email == model.Email);
                if (bibliotecario != null)
                {
                    using (var hmac = new HMACSHA512(bibliotecario.PasswordSalt))
                    {
                        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                        if (!computedHash.SequenceEqual(bibliotecario.PasswordHash))
                        {
                            ModelState.AddModelError("", "Utilizador ou password inválidos.");
                            return View(model);
                        }
                    }

                    if (bibliotecario.IsBloqueado)
                    {
                        ModelState.AddModelError("", "A sua conta está bloqueada. Contacte o suporte para mais informações.");
                        return View(model);
                    }

                    HttpContext.Session.SetString("UserName", bibliotecario.Nome);
                    HttpContext.Session.SetInt32("UserId", bibliotecario.ID_Bib);
                    HttpContext.Session.SetString("UserRole", "Bibliotecario");

                    return RedirectToAction("Index", "Bibliotecario");
                }

                // Verificar na tabela Administradores
                var admin = _context.Administrador.SingleOrDefault(a => a.Email == model.Email);
                if (admin != null)
                {
                    using (var hmac = new HMACSHA512(admin.PasswordSalt))
                    {
                        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                        if (!computedHash.SequenceEqual(admin.PasswordHash))
                        {
                            ModelState.AddModelError("", "Utilizador ou password inválidos.");
                            return View(model);
                        }
                    }

                    HttpContext.Session.SetString("UserName", admin.Nome);
                    HttpContext.Session.SetInt32("UserId", admin.ID_Admin);
                    HttpContext.Session.SetString("UserRole", "Administrador");

                    return RedirectToAction("Index", "Administrador");
                }

                // Se não encontrar o utilizador em nenhuma tabela
                ModelState.AddModelError("", "Utilizador ou password inválidos.");
                return View(model);
            }

            return View(model);
        }



        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Logout()
        {
            // Limpar a sessão do utilizador
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}