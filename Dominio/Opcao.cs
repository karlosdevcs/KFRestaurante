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

        // pedidos ja feitos nao mudam: o ItemDoPedido guarda a foto (nome e preco) do momento da compra
        public void Alterar(string nome, Categoria categoria, decimal preco)
        {
            this.Nome = nome;
            this.Categoria = categoria;
            this.Preco = preco;
        }

        public void AlterarEsgotado(bool esgotado)
        {
            this.Esgotado = esgotado; // marcar o que ja esta marcado nao da erro (idempotente)
        }

        private Opcao() { }

    }
}
