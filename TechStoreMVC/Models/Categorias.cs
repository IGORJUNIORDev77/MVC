namespace TechStoreMVC.Models;

public class Categoria
{
    // Propriedades
    public int Id {get; set;} 
    public string Nome {get; set;}
    public string Descricao {get; set;}

    // Construtor
     public Categoria()
    {
    }

    public Categoria(int id, string nome, string descricao)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
    }
}
