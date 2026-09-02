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


            Console.WriteLine(item.Subtotal);

            Pedido pedido1 = new Pedido(1, FormadePagamento.Pix, TipoDeEntrega.Entrega);
            pedido1.AdicionarItem(item);
            pedido1.AdicionarItem(item2);

            Console.WriteLine(pedido1.CalcularTotal());

        }

    }
}
