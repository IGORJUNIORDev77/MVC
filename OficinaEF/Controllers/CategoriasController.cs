using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;
namespace OficinaEF.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriasController(ApplicationDbContext contexto) : Controller
{
    // GET: /Categorias — abre a listagem.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categorias = await contexto.Categorias.AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();
        return View(categorias);
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var categoria = await contexto.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null) return NotFound();
        return View(categoria);
    }
    // GET: /Categorias/Create — abre o formulário de cadastro.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Categoria());
    }
    // POST: recebe os dados enviados pelo formulário e salva o cadastro.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Descricao")] Categoria categoria)
    {
        if (!ModelState.IsValid) return View(categoria);
        contexto.Categorias.Add(categoria);
        await contexto.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await contexto.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();
        return View(categoria);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao")] Categoria entrada)
    {
        if (id != entrada.Id) return NotFound();
        if (!ModelState.IsValid) return View(entrada);
        var categoria = await contexto.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();
        categoria.Nome = entrada.Nome;
        categoria.Descricao = entrada.Descricao;
        try { await contexto.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await contexto.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null) return NotFound();
        return View(categoria);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await contexto.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();
        if (await contexto.Produtos.AnyAsync(p => p.CategoriaId == id))
        {
            TempData["Mensagem"] = "A categoria possui produtos. Altere os vínculos antes de excluir.";
            return RedirectToAction(nameof(Index));
        }
        contexto.Categorias.Remove(categoria);
        try { await contexto.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Mensagem"] = "A categoria passou a ter um vínculo. Atualize a listagem."; }
        return RedirectToAction(nameof(Index));
    }
}