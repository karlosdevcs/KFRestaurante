using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Pedido

    {
        public int Numero { get; set; }
        public DateTime HoraPedido { get; set; }
        public FormadePagamento FormaDePagamento { get; set; }
        
        public TipoDeEntrega TipoDeEntrega { get; set; }

        private List<ItemDoPedido> _itens = new List<ItemDoPedido>();
        public IEnumerable<ItemDoPedido> Itens => _itens;

        public void AdicionarItem(ItemDoPedido item)
        {
            _itens.Add(item);
        }

        public Pedido(int numero, FormadePagamento formadepagamento, TipoDeEntrega tipodeentrega) 
        
        {
            this.Numero = numero;
            this.HoraPedido = DateTime.Now;
            this.FormaDePagamento = formadepagamento;
            this.TipoDeEntrega = tipodeentrega;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (ItemDoPedido ip in _itens)
            {
                total += ip.Subtotal;
            }
            return total;
        }



    }
}
