using Microsoft.AspNetCore.Mvc;
using LEIUTAD.Data;
using LEIUTAD.Filters;
using LEIUTAD.Models.Administrador;
using LEIUTAD.Models;
using System.Security.Cryptography;
using System.Text;
using LEIUTAD.ViewModels;

namespace LEIUTAD.Controllers
{
    [AdministradorAuthorize] // Filtro aplicado ao controlador inteiro
    public class AdministradorController : Controller
    {
        private readonly LEIUTADContext _context;

        public AdministradorController(LEIUTADContext context)
        {
            _context = context;
        }

        // Painel inicial do administrador
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = "Bem-vindo ao Painel do Administrador!";
            return View();
        }
        [HttpGet]
        public IActionResult GerirUtilizadores()
        {
            var leitores = _context.Leitor
                .Select(l => new UtilizadorViewModel
                {
                    ID = l.ID_user,
                    Nome = l.Nome,
                    Cargo = "Leitor",
                    IsBloqueado = l.IsBloqueado
                });

            var bibliotecarios = _context.Bibliotecarios
                .Select(b => new UtilizadorViewModel
                {
                    ID = b.ID_Bib,
                    Nome = b.Nome,
                    Cargo = "Bibliotecário",
                    IsBloqueado = b.IsBloqueado
                });

            var administradores = _context.Administrador
                .Select(a => new UtilizadorViewModel
                {
                    ID = a.ID_Admin,
                    Nome = a.Nome,
                    Cargo = "Administrador",
                    IsBloqueado = false // Administradores não podem ser bloqueados
                });

            var utilizadores = leitores
                .Union(bibliotecarios)
                .Union(administradores) // Adicionar os administradores
                .ToList();

            return View(utilizadores);
        }


        [HttpGet]
        public IActionResult DetalhesUtilizador(int id, string tipo)
        {
            if (tipo == "Leitor")
            {
                var leitor = _context.Leitor.FirstOrDefault(l => l.ID_user == id);
                if (leitor == null)
                {
                    return Json(new { sucesso = false, mensagem = "Leitor não encontrado." });
                }

                return PartialView("_DetalhesUtilizadorModal", new
                {
                    Tipo = "Leitor",
                    Detalhes = leitor
                });
            }
            else if (tipo == "Bibliotecário")
            {
                var bibliotecario = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == id);
                if (bibliotecario == null)
                {
                    return Json(new { sucesso = false, mensagem = "Bibliotecário não encontrado." });
                }

                return PartialView("_DetalhesUtilizadorModal", new
                {
                    Tipo = "Bibliotecário",
                    Detalhes = bibliotecario
                });
            }
            else if (tipo == "Administrador")
            {
                var administrador = _context.Administrador.FirstOrDefault(a => a.ID_Admin == id);
                if (administrador == null)
                {
                    return Json(new { sucesso = false, mensagem = "Administrador não encontrado." });
                }

                return PartialView("_DetalhesUtilizadorModal", new
                {
                    Tipo = "Administrador",
                    Detalhes = administrador
                });
            }
            else
            {
                return Json(new { sucesso = false, mensagem = "Tipo de utilizador inválido." });
            }
        }

        [HttpPost]
        public IActionResult BloquearUtilizador(int id, string cargo)
        {
            if (cargo == "Leitor")
            {
                var leitor = _context.Leitor.FirstOrDefault(l => l.ID_user == id);
                if (leitor == null) return Json(new { sucesso = false, mensagem = "Leitor não encontrado." });

                // Bloquear o leitor
                leitor.IsBloqueado = true;

                // Salvar mudanças na base de dados
                _context.SaveChanges();

                return Json(new { sucesso = true });
            }
            else if (cargo == "Bibliotecário")
            {
                var bibliotecario = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == id);
                if (bibliotecario == null) return Json(new { sucesso = false, mensagem = "Bibliotecário não encontrado." });

                // Bloquear o bibliotecário
                bibliotecario.IsBloqueado = true;

                // Salvar mudanças na base de dados
                _context.SaveChanges();

                return Json(new { sucesso = true });
            }

            return Json(new { sucesso = false, mensagem = "Tipo de utilizador inválido." });
        }



        [HttpPost]
        public IActionResult ToggleBloqueio(int id, string cargo)
        {
            if (cargo == "Leitor")
            {
                var leitor = _context.Leitor.FirstOrDefault(l => l.ID_user == id);
                if (leitor == null)
                    return Json(new { sucesso = false, mensagem = "Leitor não encontrado." });

                leitor.IsBloqueado = !leitor.IsBloqueado; // Alterna o estado de bloqueio
                _context.SaveChanges();

                return Json(new { sucesso = true, isBloqueado = leitor.IsBloqueado });
            }
            else if (cargo == "Bibliotecário")
            {
                var bibliotecario = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == id);
                if (bibliotecario == null)
                    return Json(new { sucesso = false, mensagem = "Bibliotecário não encontrado." });

                bibliotecario.IsBloqueado = !bibliotecario.IsBloqueado; // Alterna o estado de bloqueio
                _context.SaveChanges();

                return Json(new { sucesso = true, isBloqueado = bibliotecario.IsBloqueado });
            }
            else
            {
                return Json(new { sucesso = false, mensagem = "Tipo de utilizador inválido." });
            }
        }

        [HttpGet]
        public IActionResult EditarUtilizador(int id, string tipo)
        {
            if (tipo == "Leitor")
            {
                var leitor = _context.Leitor.FirstOrDefault(l => l.ID_user == id);
                if (leitor == null)
                {
                    TempData["MensagemErro"] = "Leitor não encontrado.";
                    return RedirectToAction("GerirUtilizadores");
                }
                return View("EditarLeitor", leitor); // Redireciona para a view de edição de leitores
            }
            else if (tipo == "Bibliotecário")
            {
                var bibliotecario = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == id);
                if (bibliotecario == null)
                {
                    TempData["MensagemErro"] = "Bibliotecário não encontrado.";
                    return RedirectToAction("GerirUtilizadores");
                }
                return View("EditarBibliotecario", bibliotecario); // Redireciona para a view de edição de bibliotecários
            }

            TempData["MensagemErro"] = "Tipo de utilizador inválido.";
            return RedirectToAction("GerirUtilizadores");
        }

        [HttpPost]
        public IActionResult EditarLeitor(Leitor leitorAtualizado)
        {
            try
            {
                // Verificar se o leitor existe na base de dados
                var leitorExistente = _context.Leitor.FirstOrDefault(l => l.ID_user == leitorAtualizado.ID_user);

                if (leitorExistente == null)
                {
                    TempData["MensagemErro"] = "Leitor não encontrado.";
                    return RedirectToAction("GerirUtilizadores");
                }

                // Atualizar os campos do leitor
                leitorExistente.Nome = leitorAtualizado.Nome;
                leitorExistente.Email = leitorAtualizado.Email;
                leitorExistente.Tele_n = leitorAtualizado.Tele_n;
                leitorExistente.Data_n = leitorAtualizado.Data_n;
                leitorExistente.Endereco = leitorAtualizado.Endereco;
                leitorExistente.Endereco_n = leitorAtualizado.Endereco_n;
                leitorExistente.Localidade = leitorAtualizado.Localidade;
                leitorExistente.Pais = leitorAtualizado.Pais;

                // Salvar as alterações na base de dados
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Leitor atualizado com sucesso!";
                return RedirectToAction("GerirUtilizadores");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar o leitor: {ex.Message}";
                return RedirectToAction("GerirUtilizadores");
            }
        }


        [HttpPost]
        public IActionResult EditarBibliotecario(Bibliotecarios bibliotecarioAtualizado)
        {
            try
            {
                // Verificar se o bibliotecário existe na base de dados
                var bibliotecarioExistente = _context.Bibliotecarios.FirstOrDefault(b => b.ID_Bib == bibliotecarioAtualizado.ID_Bib);

                if (bibliotecarioExistente == null)
                {
                    TempData["MensagemErro"] = "Bibliotecário não encontrado.";
                    return RedirectToAction("GerirUtilizadores");
                }

                // Atualizar os campos do bibliotecário
                bibliotecarioExistente.Nome = bibliotecarioAtualizado.Nome;
                bibliotecarioExistente.Email = bibliotecarioAtualizado.Email;
                bibliotecarioExistente.Tele_n = bibliotecarioAtualizado.Tele_n;

                // Salvar as alterações na base de dados
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Bibliotecário atualizado com sucesso!";
                return RedirectToAction("GerirUtilizadores");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar o bibliotecário: {ex.Message}";
                return RedirectToAction("GerirUtilizadores");
            }
        }


        [HttpGet]
        public IActionResult AdicionarAdministrador()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdicionarAdministrador(AdministradorViewModel novoAdmin)
        {
            // Verificar se já existe um administrador com o mesmo email
            if (_context.Administrador.Any(a => a.Email == novoAdmin.Email))
            {
                TempData["MensagemErro"] = "Já existe um administrador registado com este email.";
                return RedirectToAction("GerirUtilizadores");
            }

            // Criar o objeto Administradores e configurar os campos
            var administrador = new Administradores
            {
                Nome = novoAdmin.Nome,
                Email = novoAdmin.Email
            };

            // Gerar o hash e o salt da senha
            using (var hmac = new HMACSHA512())
            {
                administrador.PasswordSalt = hmac.Key;
                administrador.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(novoAdmin.Password));
            }

            // Adicionar o novo administrador à base de dados
            _context.Administrador.Add(administrador);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Administrador adicionado com sucesso!";
            return RedirectToAction("GerirUtilizadores");
        }



    }
}
