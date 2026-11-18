using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;

var builder = WebApplication.CreateBuilder(args);
var conexao = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure DefaultConnection.");
builder.Services.AddDbContext<ApplicationDbContext>(opcoes => opcoes.UseSqlServer(conexao));
builder.Services.AddDefaultIdentity<IdentityUser>(opcoes =>
{
    // Na aula local, a conta pode entrar logo após o cadastro.
    opcoes.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
    opcoes.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<RequestLocalizationOptions>(opcoes =>
    opcoes.SetDefaultCulture("pt-BR").AddSupportedCultures("pt-BR").AddSupportedUICultures("pt-BR"));
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Erro");
    app.UseHsts();
}
app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();
// Executado em Development quando o professor configura o ID da conta administrativa.
if (app.Environment.IsDevelopment() && app.Configuration["Loja:AdminUserId"] is { Length: > 0 } adminId)
{
    using var escopo = app.Services.CreateScope();
    var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roles = escopo.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var conta = await usuarios.FindByIdAsync(adminId)
        ?? throw new InvalidOperationException("Loja:AdminUserId não corresponde a uma conta cadastrada.");
    if (!await roles.RoleExistsAsync("Admin"))
    {
        var resultado = await roles.CreateAsync(new IdentityRole("Admin"));
        if (!resultado.Succeeded) throw new InvalidOperationException("Falha ao criar Admin.");
    }
    if (!await usuarios.IsInRoleAsync(conta, "Admin"))
    {
        var resultado = await usuarios.AddToRoleAsync(conta, "Admin");
        if (!resultado.Succeeded) throw new InvalidOperationException("Falha ao atribuir Admin.");
    }
}
app.Run();