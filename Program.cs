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
        new MySqlServerVersion(new Version(8, 0, 36)) // Ajuste a versão do MySQL se necessário
    )
);

// Adicionar serviços ao container.
builder.Services.AddControllersWithViews();

// >>> INÍCIO DA CONFIGURAÇÃO DE AUTENTICAÇÃO POR COOKIES <<<
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duração da sessão
        options.LoginPath = "/Account/Login";         // Página para onde redirecionar se não estiver logado
        options.AccessDeniedPath = "/Account/AccessDenied"; // Página para onde redirecionar se acesso for negado (logado mas sem permissão)
        options.SlidingExpiration = true; // Renova o cookie se o utilizador estiver ativo
    });
// >>> FIM DA CONFIGURAÇÃO DE AUTENTICAÇÃO POR COOKIES <<<

// **********************************************************************
// *****         ADICIONAR ESTAS DUAS LINHAS ABAIXO                 *****
// **********************************************************************
builder.Services.AddTransient<IEmailSender, EmailSender>(); // Regista o serviço de email
builder.Services.AddLogging(); // Garante que os serviços de logging estão disponíveis (ILogger)
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