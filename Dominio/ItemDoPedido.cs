using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KFRestaurante.Dominio
{
    public class ItemDoPedido
    {
        public IPedivel Pedivel { get; private set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; private set; }

        public decimal Subtotal => Quantidade * PrecoUnitario;


        public ItemDoPedido(IPedivel pedivel, int quantidade)
        {
            this.PrecoUnitario = pedivel.Preco;
            this.Pedivel = pedivel;
            this.Quantidade = quantidade;
          
        }
    }
}
