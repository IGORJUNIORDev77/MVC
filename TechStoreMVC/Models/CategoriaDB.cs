namespace TechStoreMVC.Models;
public class CategoriasDB
{
    private static readonly List<Categoria> categorias = new()
    {
         new Categoria(1, "Informática", "Computadores e componentes"),
         new Categoria(2, "Periféricos", "Mouse, teclado e acessórios"),
         new Categoria(3, "Áudio", "Fones, caixas e microfones"),
         new Categoria(4, "Monitores", "Telas e monitores"),
         new Categoria(5, "Descontinuados", "Categorias fora de uso"),
    };
    
    public List<Categoria> Listar()
    {
         return categorias;
    }  

    public Categoria? buscarPorId(int id)
    {
        return categorias.FirstOrDefault(
            categoria => categoria.Id == id
        );
    }
    public void Adicionar(Categoria categoria)
    {
        categoria.Id = categorias.Count + 1;
        categorias.Add(categoria);
    }
    public bool Atualizar(Categoria categoriaAtualizado)
    {
        Categoria? categoriaExistente =
        buscarPorId(categoriaAtualizado.Id);

        if(categoriaExistente is null)
        return false;

        categoriaExistente.Nome = categoriaAtualizado.Nome;
        categoriaExistente.Descricao = categoriaAtualizado.Descricao;
        return true;
    }

    public bool Excluir(int id)
    {
        Categoria? categoria = buscarPorId(id);
          if(categoria is null)
          return false;
        categorias.Remove(categoria);

        return true;
    }
}