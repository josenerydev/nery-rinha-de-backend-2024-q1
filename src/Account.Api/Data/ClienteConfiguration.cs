using Account.Api.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Api.Data
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes", "public");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Limite)
                .HasColumnName("limite")
                .IsRequired();

            builder.Property(c => c.Saldo)
                .HasColumnName("saldo")
                .IsRequired();

            builder.HasMany(c => c.Transacoes)
                .WithOne(t => t.Cliente)
                .HasForeignKey(t => t.ClienteId);
        }
    }
}