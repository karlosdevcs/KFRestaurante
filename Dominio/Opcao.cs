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
        // null = nao e vendida avulsa, so entra como componente de marmita (la vale o preco do tamanho)
        public decimal? Preco { get; private set; }

        public bool VendidaAvulsa => Preco != null;

        // trava de seguranca: o Pedido ja recusa opcao sem preco antes de chegar aqui.
        // Se algum codigo esquecer essa checagem, estoura em vez de vender por R$ 0,00 em silencio.
        decimal IPedivel.Preco => Preco ?? throw new InvalidOperationException($"A opção {Nome} não tem preço e não pode ser vendida avulsa.");

        public Opcao(string nome, Categoria categoria, decimal? preco = null)
        {
            Validar(nome, categoria, preco);

            this.Nome = nome;
            this.Categoria = categoria;
            this.Preco = preco;
        }

        // pedidos ja feitos nao mudam: o ItemDoPedido guarda a foto (nome e preco) do momento da compra
        public void Alterar(string nome, Categoria categoria, decimal? preco)
        {
            Validar(nome, categoria, preco);

            this.Nome = nome;
            this.Categoria = categoria;
            this.Preco = preco;
        }

        private static void Validar(string nome, Categoria categoria, decimal? preco)
        {
            if (preco < 0)
            {
                throw new DominioException($"O preço de {nome} não pode ser negativo. Valor recebido: {preco}.");
            }

            // bebida e sobremesa nao entram em marmita: sem preco, nao teria como serem vendidas
            if (!categoria.EhComponenteDeMarmita() && preco == null)
            {
                throw new DominioException($"{nome} é {categoria} e precisa ter preço, porque só pode ser vendido avulso.");
            }
        }

        public void AlterarEsgotado(bool esgotado)
        {
            this.Esgotado = esgotado; // marcar o que ja esta marcado nao da erro (idempotente)
        }

        private Opcao() { }

    }
}
