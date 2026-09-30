using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KFRestaurante.Dominio
{
    public class Pedido 

    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime HoraPedido { get; set; }
        public FormadePagamento FormaDePagamento { get; set; }
        
        public TipoDeEntrega TipoDeEntrega { get; set; }

        public CardapioDoDia CardapioDoDia { get; private set; } = null!;

        private List<ItemDoPedido> _itens = new List<ItemDoPedido>();
        public IEnumerable<ItemDoPedido> Itens => _itens;

        public void AdicionarItem(IPedivel pedivel, int quantidade)
        {

            if (pedivel is Marmita marmita) //o tipo disso e marmita? se sim, faz o foreach guardando na variavel 
            {
                foreach (Opcao c in marmita.Opcoes)
                {
                  if (!CardapioDoDia.Opcoes.Any(o => o.Nome == c.Nome))// dentro do Any, o o é cada opção do cardápio, e o c é o componente da vez
                    {
                        throw new InvalidOperationException("Esse componente nao esta disponivel hoje");
                    }
                   
                }

            } else
            {
                if (!CardapioDoDia.Opcoes.Any(o => o.Nome == pedivel.Nome))
                {
                    throw new InvalidOperationException("Esse item nao existe na lista de Opcoes");
                }
            }

            ItemDoPedido item = new ItemDoPedido(pedivel, quantidade);

             _itens.Add(item);
            

        }

        public Pedido(int numero, FormadePagamento formadepagamento, TipoDeEntrega tipodeentrega, Restaurante restaurante, CardapioDoDia cardapio)
        
        {
            

            if (!restaurante.Aberto)
            {
                throw new InvalidOperationException("O restaurante esta fechado");
            } 
            if (cardapio.Situacao != SituacaoCardapio.Publicado)
            {
                throw new InvalidOperationException("O cardapio nao esta publicado");
            }

            this.Numero = numero;
            this.HoraPedido = DateTime.Now;
            this.FormaDePagamento = formadepagamento;
            this.TipoDeEntrega = tipodeentrega;
            this.CardapioDoDia = cardapio;
           
        }

        public decimal CalcularTotal()
        {
            return _itens.Sum(ip => ip.Subtotal);
        }

        private Pedido() { }


    }
}
