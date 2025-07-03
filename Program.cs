// Ficheiro: Program.cs (Com logging para depura��o na Render)

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Necess�rio para LogLevel
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(5, 7, 25))
    )
    // <<< ALTERA��O PRINCIPAL AQUI >>>
    // Isto vai fazer o Entity Framework escrever todas as queries SQL para a Consola.
    // A Render.com captura a sa�da da Consola e mostra-a na aba "Logs" do teu servi�o.
    // � a forma mais eficaz de depurar o que est� a acontecer na base de dados.
    .LogTo(Console.WriteLine, LogLevel.Information)
);

// Resto dos teus servi�os
builder.Services.AddControllersWithViews();

// --- CONFIGURA��O DA SESS�O ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// --- FIM DA CONFIGURA��O DA SESS�O ---

// Configura��o da Autentica��o por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

// Resto dos teus servi�os
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();

// =========================================================================
// Constru��o da Aplica��o
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

// A ordem est� correta
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();