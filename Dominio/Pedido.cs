using System;
using System.Collections.Generic;
using System.Globalization;
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

        // tudo abaixo existe para a loja nao precisar perguntar nada no WhatsApp
        public string NomeCliente { get; private set; } = null!;
        public string Telefone { get; private set; } = null!;  // so digitos, com DDD: 31999999999
        public decimal? TrocoPara { get; private set; }     // so em dinheiro; null = nao precisa de troco
        public string? Referencia { get; private set; }     // so em entrega: "portao azul, ao lado da padaria"
        public string? Observacao { get; private set; }     // livre: "sem cebola"

        public CardapioDoDia CardapioDoDia { get; private set; } = null!;

        private List<ItemDoPedido> _itens = new List<ItemDoPedido>();
        public IEnumerable<ItemDoPedido> Itens => _itens;

        public void AdicionarItem(IPedivel pedivel, int quantidade)
        {

            if (pedivel is Marmita marmita) //o tipo disso e marmita? se sim, faz o foreach guardando na variavel
            {
                // a marmita nasce vazia e vai sendo montada; so quando entra no pedido ela precisa estar completa
                if (!marmita.Opcoes.Any())
                {
                    throw new DominioException($"A {marmita.Nome} precisa ter ao menos um componente.");
                }

                foreach (Opcao c in marmita.Opcoes)
                {
                  if (!CardapioDoDia.Opcoes.Any(o => o.Nome == c.Nome))// dentro do Any, o o é cada opção do cardápio, e o c é o componente da vez
                    {
                        throw new DominioException($"{c.Nome} não está no cardápio de {CardapioDoDia.DataPublicacao:dd/MM/yyyy}.");
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
                    throw new DominioException($"{pedivel.Nome} não está no cardápio de {CardapioDoDia.DataPublicacao:dd/MM/yyyy}.");
                }

                if (pedivel is Opcao opcao && opcao.Esgotado)
                {
                    throw new DominioException($"A opção {opcao.Nome} está esgotada.");
                }

                if (pedivel is Opcao semPreco && !semPreco.VendidaAvulsa)
                {
                    throw new DominioException($"{semPreco.Nome} não é vendido avulso, só como componente de marmita.");
                }
            }

            ItemDoPedido item = new ItemDoPedido(pedivel, quantidade);

             _itens.Add(item);
            

        }

        // Numero nao vem de fora: o banco gera pela sequence pedidos_numero_seq no INSERT (ver KFContext)
        public Pedido(string nomeCliente, string telefone, FormadePagamento formadepagamento, decimal? trocoPara, TipoDeEntrega tipodeentrega, string? endereco, string? referencia, string? observacao, Restaurante restaurante, CardapioDoDia cardapio)

        {
            if (string.IsNullOrWhiteSpace(nomeCliente))
            {
                throw new DominioException("Informe o nome do cliente.");
            }

            // aceita "(31) 99999-9999" e guarda "31999999999"
            string digitosTelefone = new string((telefone ?? "").Where(char.IsDigit).ToArray());

            if (digitosTelefone.Length < 10 || digitosTelefone.Length > 11)
            {
                throw new DominioException($"Telefone inválido: '{telefone}'. Informe com DDD, ex.: (31) 99999-9999.");
            }

            if (!restaurante.Aberto)
            {
                throw new DominioException("O restaurante está fechado e não está recebendo pedidos.");
            }
            if (cardapio.Situacao != SituacaoCardapio.Publicado)
            {
                throw new DominioException($"O cardápio de {cardapio.DataPublicacao:dd/MM/yyyy} não está aberto para pedidos. Situação atual: {cardapio.Situacao}.");
            }
            if (tipodeentrega == TipoDeEntrega.Entrega && string.IsNullOrWhiteSpace(endereco))
            {
                throw new DominioException("O endereço é obrigatório para pedidos com entrega.");
            }

            this.HoraPedido = DateTime.UtcNow; // UTC: o Postgres (timestamptz) nao aceita hora local
            this.FormaDePagamento = formadepagamento;
            this.TipoDeEntrega = tipodeentrega;
            this.Endereco = tipodeentrega == TipoDeEntrega.Entrega ? endereco!.Trim() : null; // retirada nao guarda endereco
            this.Referencia = tipodeentrega == TipoDeEntrega.Entrega ? TextoOuNulo(referencia) : null;
            this.TrocoPara = formadepagamento == FormadePagamento.Dinheiro ? trocoPara : null; // pix/cartao nao tem troco
            this.NomeCliente = nomeCliente.Trim();
            this.Telefone = digitosTelefone;
            this.Observacao = TextoOuNulo(observacao);
            this.CardapioDoDia = cardapio;

        }

        // chamado depois de adicionar todos os itens: so ai da para conferir o que depende do total
        public void Finalizar()
        {
            if (_itens.Count == 0)
            {
                throw new DominioException("O pedido precisa ter ao menos um item.");
            }

            decimal total = CalcularTotal();

            if (TrocoPara != null && TrocoPara < total)
            {
                CultureInfo ptBr = CultureInfo.GetCultureInfo("pt-BR"); // R$ 65,00 mesmo se o servidor estiver em ingles
                throw new DominioException($"Troco para R$ {TrocoPara.Value.ToString("N2", ptBr)} é menor que o total de R$ {total.ToString("N2", ptBr)}.");
            }
        }

        public decimal CalcularTotal()
        {
            return _itens.Sum(ip => ip.Subtotal);
        }

        private static string? TextoOuNulo(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
        }

        private Pedido() { }


    }
}
