using Microsoft.AspNetCore.Mvc;
using TechStoreMVC.Models;

namespace TechStoreMVC.Controllers
{
    public class ProdutosController : Controller // o ":" significa herança
    // Herança = herda as caracteristicas de um Controller. :)
    // Conceito de POO -> Programaçao Orientada a Objetos
    {
        // GET: ProdutosController

        // Passo 02- Criar a ação
        // Uma view depende de uma ação e vice-versa
        public ActionResult Index()
        {
            //Passo 05 - Objeto
            //Criar um objeto com varios produtos, por isso usamos um list
            List<Produto> produtos = new List<Produto>
            {
                new Produto(1, "Mouse Gamer", 129.9m, 10),
                new Produto(2, "Teclado Mecânico", 249.9m, 5),
                new Produto(3, "Monitor 24 polegadas", 899.9m, 8),
                new Produto(4, "Headset Gamer", 179.9m, 15),

            };
            return View();
        }

    }
}
