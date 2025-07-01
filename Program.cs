// Ficheiro: Program.cs (Vers�o Simples e Funcional)

// N�O TEMOS AQUELE USING COMPLICADO AQUI
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o DbContext como era no in�cio, mas com o "truque"
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        // <<< A SOLU��O SIMPLES EST� AQUI
        // Em vez de "AutoDetect" ou "new Version(8,0,36)", for�amos uma vers�o antiga.
        // Isto diz ao Pomelo para gerar SQL muito simples, que a TiDB aceita
        // sem precisar daquele PROCEDURE que deu o primeiro erro.
        new MySqlServerVersion(new Version(5, 7, 25))
    )
);

// Resto do seu c�digo original, que estava perfeito
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();