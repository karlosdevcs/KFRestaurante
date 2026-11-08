using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KFRestaurante.Dominio
{
    public class ItemDoPedido
    {

        public int Id { get; set; }

        //public IPedivel Pedivel { get; private set; } o ItemDoPedido guardava uma referência viva a algo que muda, quando o correto é guardar uma foto do momento da compra.

        public string Nome { get; private set; } = null!;

        // so marmita tem: foto dos componentes no momento do pedido, ex.: "Arroz, Feijoada"
        public string? Componentes { get; private set; }

        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }

        public decimal Subtotal => Quantidade * PrecoUnitario;


        public ItemDoPedido(IPedivel pedivel, int quantidade)
        {
            if (quantidade <= 0)
            {
                throw new InvalidOperationException($"A quantidade deve ser maior que zero. Valor recebido: {quantidade}.");
            }

            this.PrecoUnitario = pedivel.Preco;
            this.Quantidade = quantidade;
            this.Nome = pedivel.Nome;

            if (pedivel is Marmita marmita)
            {
                this.Componentes = string.Join(", ", marmita.Opcoes.Select(o => o.Nome));
            }

        }

        private ItemDoPedido() { }
    }
}
