using System.ComponentModel.DataAnnotations;
namespace OficinaEF.Models;

public class Produto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Informe o nome"), StringLength(120)]
    public string Nome { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Descricao { get; set; }
    [Range(typeof(decimal), "0.01", "9999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Informe um preço maior que zero")]
    public decimal Preco { get; set; }
    [Range(0, 1000000)]
    public int Estoque { get; set; }
    [StringLength(1000), RegularExpression(@"^(https://[^\s]+|/images/[A-Za-z0-9_./-]+)$", ErrorMessage = "Use HTTPS ou um caminho /images/foto.jpg")]
    public string? ImagemUrl { get; set; }
    public bool Ativo { get; set; } = true;
    [Range(1, int.MaxValue, ErrorMessage = "Escolha uma categoria")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    [Timestamp]
    public byte[] Versao { get; set; } = [];
}