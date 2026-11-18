using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;
namespace OficinaEF.Controllers;

[Authorize]
public class PedidosController(ApplicationDbContext contexto, UserManager<IdentityUser> usuarios) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarioId = usuarios.GetUserId(User)!;
        return View(await contexto.Pedidos.AsNoTracking().Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.Id).ToListAsync());
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var usuarioId = usuarios.GetUserId(User)!;
        var consulta = contexto.Pedidos.AsNoTracking().Include(p => p.Usuario).AsQueryable();
        if (!User.IsInRole("Admin")) consulta = consulta.Where(p => p.UsuarioId == usuarioId);
        var pedido = await consulta.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null) return NotFound();
        return View(pedido);
    }
    [HttpGet]
    public async Task<IActionResult> Create(int produtoId)
    {
        var produto = await contexto.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == produtoId && p.Ativo);
        if (produto is null) return NotFound();
        return View(new Pedido { ProdutoId = produto.Id, Produto = produto, Quantidade = 1, CodigoSolicitacao = Guid.NewGuid() });
    }
    // POST: valida a confirmação, baixa o estoque e grava o pedido.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int produtoId, int quantidade, Guid codigoSolicitacao, byte[]? versaoProduto)
    {
        var usuarioId = usuarios.GetUserId(User)!;
        var entrada = new Pedido { ProdutoId = produtoId, Quantidade = quantidade, CodigoSolicitacao = codigoSolicitacao };
        if (quantidade < 1 || quantidade > 1000000) ModelState.AddModelError("Quantidade", "Informe uma quantidade válida maior que zero.");
        if (codigoSolicitacao == Guid.Empty) ModelState.AddModelError("", "Abra novamente o formulário de pedido.");
        if (!ModelState.IsValid) return await Formulario(entrada);
        try
        {
            await using var transacao = await contexto.Database.BeginTransactionAsync();
            var existente = await contexto.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.CodigoSolicitacao == codigoSolicitacao);
            if (existente is not null)
            {
                if (existente.UsuarioId != usuarioId) return BadRequest();
                return RedirectToAction(nameof(Details), new { id = existente.Id });
            }
            var produto = await contexto.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == produtoId && p.Ativo);
            if (produto is null) return NotFound();
            if (versaoProduto is null || !produto.Versao.SequenceEqual(versaoProduto))
            {
                ModelState.AddModelError("", "O produto mudou. Confira o preço e o estoque atualizados antes de confirmar.");
                entrada.Produto = produto;
                return View(entrada);
            }
            // A condição e a baixa ocorrem juntas no SQL Server. A versão também protege o preço consultado.
            var baixados = await contexto.Produtos.Where(p => p.Id == produtoId && p.Ativo && p.Estoque >= quantidade && p.Versao == produto.Versao)
                .ExecuteUpdateAsync(atualizacao => atualizacao.SetProperty(p => p.Estoque, p => p.Estoque - quantidade));
            if (baixados == 0)
            {
                ModelState.AddModelError("", "Estoque insuficiente ou produto alterado. Confira os valores e tente novamente.");
                entrada.Produto = produto;
                return View(entrada);
            }
            entrada.UsuarioId = usuarioId;
            entrada.NomeProduto = produto.Nome;
            entrada.PrecoUnitario = produto.Preco;
            entrada.ValorTotal = produto.Preco * quantidade;
            contexto.Pedidos.Add(entrada);
            await contexto.SaveChangesAsync();
            await transacao.CommitAsync();
            TempData["Mensagem"] = "Pedido realizado com sucesso.";
            return RedirectToAction(nameof(Details), new { id = entrada.Id });
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is SqlException { Number: 1205 or 1222 })
        {
            contexto.ChangeTracker.Clear();
            ModelState.AddModelError("", "Não foi possível concluir agora. Confira o pedido e tente novamente.");
            return await Formulario(entrada);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Gestao()
    {
        return View(await contexto.Pedidos.AsNoTracking().Include(p => p.Usuario).OrderByDescending(p => p.Id).ToListAsync());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuarioId = usuarios.GetUserId(User)!;
        var consulta = contexto.Pedidos.AsNoTracking().AsQueryable();
        if (!User.IsInRole("Admin")) consulta = consulta.Where(p => p.UsuarioId == usuarioId);
        var pedido = await consulta.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null) return NotFound();
        try
        {
            await using var transacao = await contexto.Database.BeginTransactionAsync();
            var alterados = await consulta.Where(p => p.Id == id && p.Status == StatusPedido.Recebido)
                .ExecuteUpdateAsync(atualizacao => atualizacao.SetProperty(p => p.Status, StatusPedido.Cancelado));
            if (alterados == 1)
            {
                var repostos = await contexto.Produtos.Where(p => p.Id == pedido.ProdutoId)
                    .ExecuteUpdateAsync(atualizacao => atualizacao.SetProperty(p => p.Estoque, p => p.Estoque + pedido.Quantidade));
                if (repostos != 1) throw new InvalidOperationException("Produto do pedido não encontrado.");
                await transacao.CommitAsync();
                TempData["Mensagem"] = "Pedido cancelado e estoque devolvido.";
            }
            else TempData["Mensagem"] = "Este pedido já foi cancelado ou concluído.";
        }
        catch (SqlException ex) when (ex.Number is 1205 or 1222)
        {
            TempData["Mensagem"] = "O pedido está sendo atualizado. Tente novamente.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }
    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Concluir(int id)
    {
        var alterados = await contexto.Pedidos.Where(p => p.Id == id && p.Status == StatusPedido.Recebido)
            .ExecuteUpdateAsync(atualizacao => atualizacao.SetProperty(p => p.Status, StatusPedido.Concluido));
        TempData["Mensagem"] = alterados == 1 ? "Pedido concluído." : "O pedido não está disponível para conclusão.";
        return RedirectToAction(nameof(Details), new { id });
    }
    private async Task<IActionResult> Formulario(Pedido entrada)
    {
        entrada.Produto = await contexto.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entrada.ProdutoId && p.Ativo);
        if (entrada.Produto is null) return NotFound();
        return View("Create", entrada);
    }
}