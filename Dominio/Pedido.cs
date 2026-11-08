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
        public int Id { get; private set; }
        public int Numero { get; private set; }
        public DateTime HoraPedido { get; private set; }
        public FormadePagamento FormaDePagamento { get; private set; }
        
        public TipoDeEntrega TipoDeEntrega { get; private set; }

        public string? Endereco { get; private set; }

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
                        throw new DominioException("Esse componente nao esta disponivel hoje");
                    }

                    if (c.Esgotado)
                    {
                        throw new DominioException($"A opção {c.Nome} está esgotada.");
                    }

                }

            } else
            {
                if (!CardapioDoDia.Opcoes.Any(o => o.Nome == pedivel.Nome))
                {
                    throw new DominioException("Esse item nao existe na lista de Opcoes");
                }

                if (pedivel is Opcao opcao && opcao.Esgotado)
                {
                    throw new DominioException($"A opção {opcao.Nome} está esgotada.");
                }
            }

            ItemDoPedido item = new ItemDoPedido(pedivel, quantidade);

             _itens.Add(item);
            

        }

        // Numero nao vem de fora: o banco gera pela sequence pedidos_numero_seq no INSERT (ver KFContext)
        public Pedido(FormadePagamento formadepagamento, TipoDeEntrega tipodeentrega, string? endereco, Restaurante restaurante, CardapioDoDia cardapio)

        {


            if (!restaurante.Aberto)
            {
                throw new DominioException("O restaurante esta fechado");
            }
            if (cardapio.Situacao != SituacaoCardapio.Publicado)
            {
                throw new DominioException("O cardapio nao esta publicado");
            }
            if (tipodeentrega == TipoDeEntrega.Entrega && string.IsNullOrWhiteSpace(endereco))
            {
                throw new DominioException("O endereço é obrigatório para pedidos com entrega.");
            }

            this.HoraPedido = DateTime.UtcNow; // UTC: o Postgres (timestamptz) nao aceita hora local
            this.FormaDePagamento = formadepagamento;
            this.TipoDeEntrega = tipodeentrega;
            this.Endereco = tipodeentrega == TipoDeEntrega.Entrega ? endereco!.Trim() : null; // retirada nao guarda endereco
            this.CardapioDoDia = cardapio;
           
        }

        public decimal CalcularTotal()
        {
            return _itens.Sum(ip => ip.Subtotal);
        }

        private Pedido() { }


    }
}
