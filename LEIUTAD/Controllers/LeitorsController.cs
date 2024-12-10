using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LEIUTAD.Data;
using LEIUTAD.Models;

namespace LEIUTAD.Controllers
{
    public class LeitorsController : Controller
    {
        private readonly LEIUTADContext _context;

        public LeitorsController(LEIUTADContext context)
        {
            _context = context;
        }

        // GET: Leitors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Leitor.ToListAsync());
        }

        // GET: Leitors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leitor = await _context.Leitor
                .FirstOrDefaultAsync(m => m.ID_user == id);
            if (leitor == null)
            {
                return NotFound();
            }

            return View(leitor);
        }

        // GET: Leitors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leitors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID_user,Nome,PasswordHash,PasswordSalt,Email,Tele_n,Data_n,Endereco,Endereco_n,Localidade,Pais")] Leitor leitor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leitor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leitor);
        }

        // GET: Leitors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leitor = await _context.Leitor.FindAsync(id);
            if (leitor == null)
            {
                return NotFound();
            }
            return View(leitor);
        }

        // POST: Leitors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_user,Nome,PasswordHash,PasswordSalt,Email,Tele_n,Data_n,Endereco,Endereco_n,Localidade,Pais")] Leitor leitor)
        {
            if (id != leitor.ID_user)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leitor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeitorExists(leitor.ID_user))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leitor);
        }

        // GET: Leitors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leitor = await _context.Leitor
                .FirstOrDefaultAsync(m => m.ID_user == id);
            if (leitor == null)
            {
                return NotFound();
            }

            return View(leitor);
        }

        // POST: Leitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leitor = await _context.Leitor.FindAsync(id);
            if (leitor != null)
            {
                _context.Leitor.Remove(leitor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeitorExists(int id)
        {
            return _context.Leitor.Any(e => e.ID_user == id);
        }
    }
}
