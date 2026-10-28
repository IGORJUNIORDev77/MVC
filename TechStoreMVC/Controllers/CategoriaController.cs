using Microsoft.AspNetCore.Mvc;
using TechStoreMVC.Models;

namespace TechStoreMVC.Controllers
{
    public class CategoriasController : Controller
    {
         private readonly CategoriasDB _categoriasDB;
    public CategoriasController()
    {
      _categoriasDB = new CategoriasDB();
    }
        public ActionResult Index()
        {
            List <Categoria> categorias = new List<Categoria>
            {
                new Categoria(1, "Informática", "Computadores e componentes"),
                new Categoria(2, "Periféricos", "Mouse, teclado e acessórios"),
                new Categoria(3, "Áudio", "Fones, caixas e microfones"),
                new Categoria(4, "Monitores", "Telas e monitores"),
                new Categoria(5, "Descontinuados", "Categorias fora de uso"),
            };
            return View(categorias);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
           Categoria? categoria = _categoriasDB.buscarPorId(id);
          if (categoria is null)
          return NotFound();

         return View(categoria);
        }
        [HttpGet]
       public IActionResult Create()
       {
        return View();
       }
      [HttpPost]
      [ValidateAntiForgeryToken]
       public IActionResult Create(Categoria categoria)
       {
        if (!ModelState.IsValid)
        return View(categoria);

      _categoriasDB.Adicionar(categoria);
      TempData["Sucesso"] =
        "Categoria cadastrada com sucesso!";
      return RedirectToAction(nameof(Index));
      }
    

     [HttpGet]
    public IActionResult Edit(int id)
    {
      Categoria? categoria = _categoriasDB.buscarPorId(id);
      if (categoria is null)
        return NotFound();
      return View(categoria);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Categoria categoria, int id)
    {
      if (id != categoria.Id)
        return BadRequest();

      if (!ModelState.IsValid)
        return View(categoria);

      bool atualizou = _categoriasDB.Atualizar(categoria);
      if (!atualizou)
        return NotFound();

      TempData["Sucesso"] =
        "Categoria atualizada com sucesso!";

      return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
      Categoria? categoria = _categoriasDB.buscarPorId(id);
      if (categoria is null)
        return NotFound();
      return View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
      bool excluiu = _categoriasDB.Excluir(id);
      if (!excluiu)
        return NotFound();

      TempData["Sucesso"] =
        "Categoria excluido com sucesso!";

      return RedirectToAction(nameof(Index));

      }
    }
}

