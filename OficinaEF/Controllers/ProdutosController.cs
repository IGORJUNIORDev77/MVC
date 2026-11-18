using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;
namespace OficinaEF.Controllers;

[Authorize(Roles = "Admin")]
public class ProdutosController(ApplicationDbContext contexto) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? busca)
    {
        var consulta = contexto.Produtos.AsNoTracking().Include(p => p.Categoria).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busca)) consulta = consulta.Where(p => p.Nome.Contains(busca));
        ViewBag.Busca = busca;
        return View(await consulta.OrderBy(p => p.Nome).ToListAsync());
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var produto = await contexto.Produtos.AsNoTracking().Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null) return NotFound();
        return View(produto);
    }
    // GET: /Produtos/Create — carrega as categorias e abre o formulário.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await Categorias();
        return View(new Produto());
    }
    // POST: recebe os dados enviados pelo formulário e salva o cadastro.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Descricao,Preco,Estoque,ImagemUrl,Ativo,CategoriaId")] Produto produto)
    {
        await ValidarCategoria(produto);
        if (!ModelState.IsValid) { await Categorias(); return View(produto); }
        contexto.Produtos.Add(produto);
        await contexto.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var produto = await contexto.Produtos.FindAsync(id);
        if (produto is null) return NotFound();
        await Categorias(); return View(produto);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Preco,Estoque,ImagemUrl,Ativo,CategoriaId,Versao")] Produto entrada)
    {
        if (id != entrada.Id) return NotFound();
        await ValidarCategoria(entrada);
        if (!ModelState.IsValid) { await Categorias(); return View(entrada); }
        var produto = await contexto.Produtos.FindAsync(id);
        if (produto is null) return NotFound();
        contexto.Entry(produto).Property(p => p.Versao).OriginalValue = entrada.Versao;
        produto.Nome = entrada.Nome;
        produto.Descricao = entrada.Descricao;
        produto.Preco = entrada.Preco;
        produto.Estoque = entrada.Estoque;
        produto.ImagemUrl = entrada.ImagemUrl;
        produto.Ativo = entrada.Ativo;
        produto.CategoriaId = entrada.CategoriaId;
        try { await contexto.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "Preço ou estoque mudou desde que você abriu a tela. Abra a edição novamente.");
            await Categorias(); return View(entrada);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await contexto.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (produto is null) return NotFound();
        return View(produto);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await contexto.Produtos.FindAsync(id);
        if (produto is null) return NotFound();
        if (await contexto.Pedidos.AnyAsync(p => p.ProdutoId == id))
        {
            produto.Ativo = false;
            TempData["Mensagem"] = "O produto tem histórico e foi desativado.";
        }
        else contexto.Produtos.Remove(produto);
        try { await contexto.SaveChangesAsync(); }
        catch (DbUpdateException) { TempData["Mensagem"] = "O produto foi alterado ou recebeu um pedido. Atualize a lista."; }
        return RedirectToAction(nameof(Index));
    }
    private async Task Categorias() => ViewBag.CategoriaId = new SelectList(await contexto.Categorias.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
    private async Task ValidarCategoria(Produto produto)
    {
        if (!await contexto.Categorias.AnyAsync(c => c.Id == produto.CategoriaId))
            ModelState.AddModelError("CategoriaId", "Escolha uma categoria cadastrada.");
    }
}