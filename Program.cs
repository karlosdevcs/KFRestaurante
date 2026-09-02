using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

         /*   TamanhoMarmita tamanhomarmita = new TamanhoMarmita("G", 25);
            Marmita marmita1 = new Marmita(tamanhomarmita);

            Console.WriteLine($"O preco da marmita e {marmita1.PrecoMarmita}");

            Opcao feijaotropeiro = new Opcao("Feijao tropeiro", Categoria.Prato);*/

            Opcao guarana = new Opcao("Guarana", Categoria.Bebida, 12);
            ItemDoPedido item = new ItemDoPedido(guarana, 2);

            Opcao suco = new Opcao("Suco de laranja", Categoria.Bebida, 3);
            ItemDoPedido item2 = new ItemDoPedido(suco, 1);



            Console.WriteLine(item.Subtotal);

            Pedido pedido1 = new Pedido(1, FormadePagamento.Pix, TipoDeEntrega.Entrega);
            pedido1.AdicionarItem(item);
            pedido1.AdicionarItem(item2);

            Console.WriteLine(pedido1.CalcularTotal());



           /* marmita1.AdicionarOpcao(feijaotropeiro);  
            
            foreach (Opcao o in marmita1.Opcoes)
            {
                Console.WriteLine($"O nome escolhido foi {o.Nome}, {o.Categoria}");
                
            }

            Console.WriteLine($"Preco da marmita e {marmita1.PrecoMarmita}");*/
        }

    }
}
