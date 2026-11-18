using System.ComponentModel.DataAnnotations;
namespace OficinaEF.Models;

public class Categoria
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Informe o nome"), StringLength(80)]
    public string Nome { get; set; } = string.Empty;
    [StringLength(300)]
    public string? Descricao { get; set; }
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}