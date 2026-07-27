using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KFRestaurante.Dominio
{
    public class ItemDoPedido
    {
        public Opcao Opcao { get; private set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; private set; }

        public decimal Subtotal => Quantidade * PrecoUnitario;


        public ItemDoPedido(Opcao opcao, int quantidade)
        {
            this.PrecoUnitario = opcao.Preco;
            this.Opcao = opcao;
            this.Quantidade = quantidade;
          
        }
    }
}
