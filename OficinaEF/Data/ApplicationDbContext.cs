using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Models;
namespace OficinaEF.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Produto>().Property(p => p.Preco).HasPrecision(10, 2);
        builder.Entity<Produto>().HasOne(p => p.Categoria).WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Pedido>().Property(p => p.PrecoUnitario).HasPrecision(10, 2);
        builder.Entity<Pedido>().Property(p => p.ValorTotal).HasPrecision(16, 2);
        builder.Entity<Pedido>().Property(p => p.NomeProduto).HasMaxLength(120);
        builder.Entity<Pedido>().HasIndex(p => p.CodigoSolicitacao).IsUnique();
        builder.Entity<Pedido>().HasOne(p => p.Produto).WithMany()
            .HasForeignKey(p => p.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Pedido>().HasOne(p => p.Usuario).WithMany()
            .HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}