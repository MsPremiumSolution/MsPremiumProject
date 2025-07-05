// MSPremiumProject.Data/DataProtectionKeysContext.cs
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore; // IMPORTE ESTE USING
using Microsoft.EntityFrameworkCore;
// Remova: using MSPremiumProject.Models; // Certifique-se que esta linha NÃO existe

namespace MSPremiumProject.Data
{
    // O seu DbContext deve implementar a interface IDataProtectionKeyContext
    public class DataProtectionKeysContext : DbContext, IDataProtectionKeyContext // <<< ALTERAÇÃO AQUI
    {
        public DataProtectionKeysContext(DbContextOptions<DataProtectionKeysContext> options)
            : base(options) { }

        // O DbSet para as chaves de Data Protection.
        // O tipo deve ser 'DataProtectionKey' do namespace Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } // <<< ESTA LINHA ESTÁ CORRETA

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Chame a implementação base primeiro

            // Opcional: Especificar o nome da tabela no DB
            // Isto garante que a tabela no MySQL será "DataProtectionKeys"
            modelBuilder.Entity<DataProtectionKey>().ToTable("DataProtectionKeys");
        }
    }
}