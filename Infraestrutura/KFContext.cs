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

            modelBuilder.Entity<Opcao>()
                .HasIndex(o => o.Nome)
                .IsUnique();

            modelBuilder.Entity<Pedido>()
                .HasIndex(p => p.Numero)
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
