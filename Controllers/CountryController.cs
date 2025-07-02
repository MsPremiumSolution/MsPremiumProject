// Ficheiro: Controllers/CountryController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MSPremiumProject.Controllers
{
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CountryController> _logger;

        public CountryController(AppDbContext context, ILogger<CountryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Country
        // Mostra a lista de todos os países
        public async Task<IActionResult> Index()
        {
            var paises = await _context.Paises.OrderBy(p => p.NomePais).ToListAsync();
            // A convenção espera uma view em Views/Country/Index.cshtml
            return View(paises);
        }

        // GET: /Country/Create
        // Mostra o formulário de criação de um novo país
        public IActionResult Create()
        {
            // A convenção espera uma view em Views/Country/Create.cshtml
            return View(new Pai());
        }

        // POST: /Country/Create
        // Processa os dados do formulário submetido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomePais")] Pai pais)
        {
            // O CodigoIso não vem do formulário, por isso removemo-lo da validação inicial.
            ModelState.Remove("CodigoIso");

            if (ModelState.IsValid)
            {
                // Normaliza o nome do país para verificação (remove espaços e converte para minúsculas)
                var nomeNormalizado = pais.NomePais.Trim().ToLower();

                // Verifica se já existe um país com o mesmo nome
                if (await _context.Paises.AnyAsync(p => p.NomePais.ToLower() == nomeNormalizado))
                {
                    ModelState.AddModelError("NomePais", "Já existe um país com este nome.");
                }
                else
                {
                    // Lógica para atribuir o CodigoIso correto
                    switch (nomeNormalizado)
                    {
                        case "portugal":
                            pais.CodigoIso = "PT";
                            break;
                        case "espanha":
                            pais.CodigoIso = "ES";
                            break;
                        case "frança":
                            pais.CodigoIso = "FR";
                            break;
                        // Adicione outros mapeamentos conhecidos aqui
                        // case "brasil":
                        //     pais.CodigoIso = "BR";
                        //     break;

                        default:
                            // Fallback para países não mapeados: usar as duas primeiras letras.
                            // Inclui uma verificação para evitar erros se o nome for muito curto.
                            if (pais.NomePais.Length >= 2)
                            {
                                pais.CodigoIso = pais.NomePais.Substring(0, 2).ToUpper();
                            }
                            else
                            {
                                // Se o nome for inválido, adiciona um erro e retorna ao formulário.
                                ModelState.AddModelError("NomePais", "O nome do país deve ter pelo menos 2 caracteres.");
                                return View(pais); // Retorna ao formulário para o utilizador corrigir
                            }
                            break;
                    }

                    // Se tudo estiver correto, tenta guardar na base de dados
                    try
                    {
                        _context.Add(pais);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"País '{pais.NomePais}' criado com sucesso com o código '{pais.CodigoIso}'.");
                        TempData["MensagemSucesso"] = $"País '{pais.NomePais}' adicionado com sucesso!";
                        return RedirectToAction(nameof(Index)); // Redireciona para a lista de países
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Erro ao criar o país '{pais.NomePais}'.");
                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao guardar os dados. Tente novamente.");
                    }
                }
            }

            // Se o ModelState não for válido (seja por validação inicial ou por nome duplicado),
            // retorna à mesma view para mostrar os erros.
            return View(pais);
        }
    }
}