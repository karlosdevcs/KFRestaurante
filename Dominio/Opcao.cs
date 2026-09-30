using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Opcao: IPedivel
    {

        public int Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public Categoria Categoria { get; private set; }
        public bool Esgotado { get; private set; }
        public decimal Preco { get; private set; }

        public Opcao(string nome, Categoria categoria, decimal preco = 0)
        {
            this.Nome = nome;
            this.Categoria = categoria;
            this.Preco = preco;
        }

        private Opcao() { }

    }
}
