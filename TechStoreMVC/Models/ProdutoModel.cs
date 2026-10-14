namespace TechStoreMVC.Models;

public class Produto
{
    // PASSO 01 - CRIAR AS MODELS
    // Atributos
    public int Id {get; set;} // Get para obter um id = Identificador
                              // Set para definir um novo valor, atualizar e etc
    public string Nome {get; set;}
    public decimal Preco {get; set;}
    public int Estoque {get; set;}

    // Método Construtor - Define valores iniciais desses atributos acima!
    public Produto (
        int id,
        string nome,
        decimal preco,
        int estoque
    )
    {
        Id = id;
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
    }
}