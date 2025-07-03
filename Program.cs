// Ficheiro: Program.cs (Com logging para depuração na Render)

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Necessário para LogLevel
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(5, 7, 25))
    )
    // <<< ALTERAÇÃO PRINCIPAL AQUI >>>
    // Isto vai fazer o Entity Framework escrever todas as queries SQL para a Consola.
    // A Render.com captura a saída da Consola e mostra-a na aba "Logs" do teu serviço.
    // É a forma mais eficaz de depurar o que está a acontecer na base de dados.
    .LogTo(Console.WriteLine, LogLevel.Information)
);

// Resto dos teus serviços
builder.Services.AddControllersWithViews();

// --- CONFIGURAÇÃO DA SESSÃO ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// --- FIM DA CONFIGURAÇÃO DA SESSÃO ---

// Configuração da Autenticação por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

// Resto dos teus serviços
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();

// =========================================================================
// Construção da Aplicação
var app = builder.Build();
// =========================================================================

// Pipeline de Pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// A ordem está correta
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();