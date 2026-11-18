using Microsoft.AspNetCore.Identity;
namespace OficinaEF.Models;

public enum StatusPedido { Recebido = 1, Concluido = 2, Cancelado = 3 }

public class Pedido
{
    public int Id { get; set; }
    public Guid CodigoSolicitacao { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser? Usuario { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    public StatusPedido Status { get; set; } = StatusPedido.Recebido;
}