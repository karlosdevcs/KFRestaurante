using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Opcao: IPedivel
    {

        public int Id { get; set; }
        public string Nome { get; set; } = null;
        public Categoria Categoria { get; set; }
        public bool Esgotado { get; set; }
        public decimal Preco { get; set; }

        public Opcao(string nome, Categoria categoria, decimal preco = 0)
        {
            this.Nome = nome;
            this.Categoria = categoria;
            this.Preco = preco;
        }

        private Opcao() { }

    }
}
