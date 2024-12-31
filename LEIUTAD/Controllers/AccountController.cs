using LEIUTAD.Data;
using LEIUTAD.Models;
using LEIUTAD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web;

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

                EnviarEmailVerificacao(leitor);
                TempData["MensagemSucesso"] = "Conta criada com sucesso! Por favor, verifique o seu email para ativar a conta.";
                return RedirectToAction("Login");

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
                // Verificar na tabela Leitor
                var user = _context.Leitor.SingleOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
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
                    HttpContext.Session.SetString("UserRole", "Leitor");

                    return RedirectToAction("Index", "Livros");
                }

                // Verificar na tabela Bibliotecarios
                var bibliotecario = _context.Bibliotecarios.SingleOrDefault(b => b.Email == model.Email);
                if (bibliotecario != null)
                {
                    // Verificar password
                    using (var hmac = new HMACSHA512(bibliotecario.PasswordSalt))
                    {
                        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                        if (!computedHash.SequenceEqual(bibliotecario.PasswordHash))
                        {
                            ModelState.AddModelError("", "Utilizador ou password inválidos.");
                            return View(model);
                        }
                    }

                    // Guardar informações de sessão
                    HttpContext.Session.SetString("UserName", bibliotecario.Nome);
                    HttpContext.Session.SetInt32("UserId", bibliotecario.ID_Bib);
                    HttpContext.Session.SetString("UserRole", "Bibliotecario");

                    return RedirectToAction("Index", "Bibliotecario");

                }

                // Se não encontrar em nenhuma tabela
                ModelState.AddModelError("", "Utilizador ou password inválidos.");
                return View(model);
            }

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

            _emailService.SendEmail(leitor.Email, subject, body); // Usa o serviço injetado
        }


        [HttpGet]
        public IActionResult VerificarConta(string token)
        {
            var user = _context.Leitor.SingleOrDefault(l => l.TokenVerificacao == token);

            if (user == null)
            {
                TempData["MensagemErro"] = "Token de verificação inválido ou expirado.";
                return RedirectToAction("Login");
            }

            user.Estado = true; // Ativa a conta
            user.TokenVerificacao = null; // Remove o token após verificação
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Conta verificada com sucesso!";
            return RedirectToAction("Login");
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
