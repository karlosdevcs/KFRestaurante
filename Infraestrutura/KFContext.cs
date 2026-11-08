using KFRestaurante.Dominio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Infraestrutura
{
    public class KFContext : DbContext
    {
        public DbSet<Opcao> Opcoes { get; set;  }
        public DbSet<TamanhoMarmita> TamanhosMarmita { get; set; }
        public DbSet<CardapioDoDia> CardapiosDoDia { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemDoPedido> ItemDoPedido { get; set; }

        public DbSet<Marmita> Marmitas { get; set; }

        public DbSet<Restaurante> Restaurantes { get; set; }

        public KFContext(DbContextOptions<KFContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Marmita>()
                .HasMany(m => m.Opcoes)
                .WithMany();

            // apagar um tamanho NAO pode apagar marmitas junto: o banco recusa enquanto houver marmita usando o tamanho
            modelBuilder.Entity<Marmita>()
                .HasOne(m => m.Tamanho)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opcao>()
                .HasIndex(o => o.Nome)
                .IsUnique();

            modelBuilder.Entity<TamanhoMarmita>()
                .HasIndex(t => t.Nome)
                .IsUnique();

            // limites por categoria: tabela propria, mas sempre carregada e salva junto com o tamanho
            modelBuilder.Entity<TamanhoMarmita>()
                .OwnsMany(t => t.Limites, limite =>
                {
                    limite.ToTable("limitescategoria");
                    limite.WithOwner().HasForeignKey("TamanhoMarmitaId");
                    limite.HasKey("TamanhoMarmitaId", "Categoria"); // um limite por categoria em cada tamanho
                });

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.Numero)
                .IsUnique();

            // numero do pedido gerado pelo banco: sequencial, sem reiniciar, sem colisao entre pedidos simultaneos
            modelBuilder.HasSequence<int>("pedidos_numero_seq");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Numero)
                .HasDefaultValueSql("nextval('pedidos_numero_seq')");

            // apagar um cardapio NAO pode apagar os pedidos daquele dia (sao o historico de vendas)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.CardapioDoDia)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardapioDoDia>()
                .HasIndex(c => c.DataPublicacao)
                .IsUnique();

            modelBuilder.Entity<CardapioDoDia>()
                .HasMany(c => c.Opcoes)
                .WithMany();

            foreach (var entidade in modelBuilder.Model.GetEntityTypes())
            {

                string? nomeDaTabela = entidade.GetTableName();

                if (nomeDaTabela != null) 
                { 
                    
                    entidade.SetTableName(nomeDaTabela.ToLower());
                
                }

                foreach (var propriedade in entidade.GetProperties())
                {
                    propriedade.SetColumnName(propriedade.GetColumnName().ToLower());
                }
            }

        }


    }
}
