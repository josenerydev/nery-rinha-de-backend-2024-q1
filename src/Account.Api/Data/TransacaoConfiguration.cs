using Account.Api.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Api.Data
{
    public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("transacoes", "public");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Valor)
                .HasColumnName("valor")
                .IsRequired();

            builder.Property(t => t.Tipo)
                .HasColumnName("tipo")
                .IsRequired()
                .HasColumnType("CHAR(1)");

            builder.Property(t => t.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(10);

            builder.Property(t => t.RealizadaEm)
                .HasColumnName("realizada_em")
                .IsRequired();

            builder.Property(t => t.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            builder.HasOne(t => t.Cliente)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.ClienteId);
        }
    }
}