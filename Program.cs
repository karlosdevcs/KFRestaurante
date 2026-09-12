using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

            Opcao guarana = new Opcao("Guarana", Categoria.Bebida, 12);
            ItemDoPedido item = new ItemDoPedido(guarana, 2);

            Opcao suco = new Opcao("Suco de laranja", Categoria.Bebida, 3);
            ItemDoPedido item2 = new ItemDoPedido(suco, 1);

            TamanhoMarmita marmita = new TamanhoMarmita("G", 25);
            Marmita marmita1 = new Marmita(marmita);

            ItemDoPedido item3 = new ItemDoPedido(marmita1, 1);

            Catalogo catalogo = new Catalogo();

            catalogo.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            try
            {
                catalogo.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            }
            catch (Exception e)
            {
                Console.WriteLine("Erro em " + e.Message);
            }

            catalogo.AdicionarOpcao(new Opcao("Batata", Categoria.Acompanhamento));

            foreach (Opcao o in catalogo.Opcoes)
            {
                Console.WriteLine($"Adicionado no catalogo {o.Nome} das categorias {o.Categoria}");
            }

            Console.WriteLine("===========INFORMACOES DO PEDIDO==========");


            //Console.WriteLine(item.Subtotal);

            Pedido pedido1 = new Pedido(001, FormadePagamento.Pix, TipoDeEntrega.Entrega);
            pedido1.AdicionarItem(item);
            pedido1.AdicionarItem(item2);
            pedido1.AdicionarItem(item3);

            // Console.WriteLine(pedido1.CalcularTotal());


            Console.WriteLine("Informacoes do Pedido");
            Console.WriteLine($"Pedido numero: {pedido1.Numero}, {pedido1.HoraPedido}, {pedido1.FormaDePagamento}");


            foreach (ItemDoPedido i in pedido1.Itens)
            {
                Console.WriteLine($"- {i.Quantidade}x - {i.Pedivel.Nome} - R$: {i.Subtotal}");

            }

            Console.WriteLine($"TOTAL R$: {pedido1.CalcularTotal()}");
            Console.WriteLine($"{pedido1.TipoDeEntrega}");

        }

    }
}
