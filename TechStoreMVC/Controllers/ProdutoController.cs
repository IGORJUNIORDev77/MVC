using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechStoreMVC.Models;

namespace TechStoreMVC.Controllers
{  
    public class ProdutoController : Controller // o ":" significa herença
    // Herança = herda as caracteristicas de um Controller.
    // Conceito de POO -> Programação Orientada a Objetos
    {
        // GET: ProdutosController
        private readonly ProdutoDB _produtosDB;
        public ProdutoController()
        {
            _produtosDB = new ProdutoDB();
        }

        // Passo 02 - CRIAR AÇÃO
        //Uma view Depende de uma ação e vice-versa
        [HttpGet] // Verbo Http -> para ver os produtos
        public ActionResult Index()
        {
            // Passo 05 - Objetivo
            // Criar um objeto com vários produtos, por isso usamos um List
            List<Produto> produtos = _produtosDB.Listar();
            return View(produtos);
        }

       [HttpGet]
       public IActionResult Details(int id)
       {
        Produto? produto = _produtosDB.buscarPorId(id);
        if (produto is null)
          return NotFound();

          return View(produto);
       }
       [HttpGet]
       public IActionResult Create()
        {
            return View();
        }
       [HttpPost]
       [ValidateAntiForgeryToken]
       public IActionResult Create(Produto produto)
        {
            if(!ModelState.IsValid)
              return View(produto);

            _produtosDB.Adicionar(produto);
            TempData["Sucesso"] = 
              "Produto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
       public IActionResult Edit(int id)
        {
            Produto? produto = _produtosDB.buscarPorId(id);
            if (produto is null)
              return NotFound();
            return View(produto);
        }

        [HttpGet]
       public IActionResult Edit()
        {
            return View();
        }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public IActionResult Edit(Produto produto, int id)
        {
            if (id != produto.Id)
             return BadRequest();

            if(!ModelState.IsValid)
              return View(produto);

            bool atualizou = _produtosDB.Atualizar(produto);
            if(!atualizou)
             return NotFound();

            TempData["Sucesso"] = 
              "Produto atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
       public IActionResult Delete(int id)
        {
            Produto? produto = _produtosDB.buscarPorId(id);
            if (produto is null)
              return NotFound();
            return View(produto);
        }
    }
}
