using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Services; // <<< ADICIONAR ESTE USING para IEmailSender e EmailSender

var builder = WebApplication.CreateBuilder(args);

// Adiciona o DbContext com MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)) // Ajuste a vers�o do MySQL se necess�rio
    )
);

// Adicionar servi�os ao container.
builder.Services.AddControllersWithViews();

// >>> IN�CIO DA CONFIGURA��O DE AUTENTICA��O POR COOKIES <<<
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Dura��o da sess�o
        options.LoginPath = "/Account/Login";         // P�gina para onde redirecionar se n�o estiver logado
        options.AccessDeniedPath = "/Account/AccessDenied"; // P�gina para onde redirecionar se acesso for negado (logado mas sem permiss�o)
        options.SlidingExpiration = true; // Renova o cookie se o utilizador estiver ativo
    });
// >>> FIM DA CONFIGURA��O DE AUTENTICA��O POR COOKIES <<<

// **********************************************************************
// *****         ADICIONAR ESTAS DUAS LINHAS ABAIXO                 *****
// **********************************************************************
builder.Services.AddTransient<IEmailSender, EmailSender>(); // Regista o servi�o de email
builder.Services.AddLogging(); // Garante que os servi�os de logging est�o dispon�veis (ILogger)
// **********************************************************************
// **********************************************************************


var app = builder.Build();

// Configure o pipeline de pedidos HTTP.
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