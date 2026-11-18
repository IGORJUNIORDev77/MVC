using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
namespace OficinaEF.Controllers;

public class HomeController(ApplicationDbContext contexto) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? busca, int? categoriaId)
    {
        var consulta = contexto.Produtos.AsNoTracking().Where(p => p.Ativo);
        if (!string.IsNullOrWhiteSpace(busca)) consulta = consulta.Where(p => p.Nome.Contains(busca));
        if (categoriaId.HasValue) consulta = consulta.Where(p => p.CategoriaId == categoriaId);
        ViewBag.Busca = busca;
        ViewBag.CategoriaId = categoriaId;
        ViewBag.Categorias = await contexto.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        return View(await consulta.Include(p => p.Categoria).OrderBy(p => p.Nome).ToListAsync());
    }
    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var produto = await contexto.Produtos.AsNoTracking().Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
        if (produto is null) return NotFound();
        return View(produto);
    }
    public IActionResult Erro() => Problem("Não foi possível concluir a operação. Tente novamente.");
}