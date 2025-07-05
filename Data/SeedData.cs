// Ficheiro: MSPremiumProject.Data/SeedData.cs

using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Para List<T>

namespace MSPremiumProject.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context)
        {
            // --- Seed para Tipos de Janela ---
            

            // --- Seed para Estados de Proposta ---
            // AJUSTADO para o seu modelo EstadoProposta.cs (EstadoPropostaId e Nome)
            if (!context.EstadoPropostas.Any())
            {
                var estados = new List<EstadoProposta>
                {
                    // É crucial que os IDs aqui correspondam aos IDs usados na sua aplicação (ESTADO_EM_CURSO = 1, ESTADO_CONCLUIDO = 2)
                    new EstadoProposta { EstadoPropostaId = 1, Nome = "Em Curso" },
                    new EstadoProposta { EstadoPropostaId = 2, Nome = "Concluído" },
                    new EstadoProposta { EstadoPropostaId = 3, Nome = "Cancelado" },
                    // Adicione mais estados se necessário
                };
                await context.EstadoPropostas.AddRangeAsync(estados);
                await context.SaveChangesAsync();
            }
        }
    }
}