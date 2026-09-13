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

            //ItemDoPedido item3 = new ItemDoPedido(marmita1, 1);

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


            Restaurante situacaoRestaurante = new Restaurante();
            /*
                        try
                        {
                            Pedido pedidoteste = new Pedido(002, FormadePagamento.CartaoDeDebito, TipoDeEntrega.Retirada, situacaoRestaurante);


                        } catch (Exception e)
                        {
                            Console.WriteLine("O restaurante precisa estar aberto para fazer pedidos. Aguarde! " + e.Message);
                        }

                        Restaurante situacaoRestaurante2 = new Restaurante();
                        situacaoRestaurante2.Aberto = true;

                        Pedido pedido2 = new Pedido(002, FormadePagamento.CartaoDeDebito, TipoDeEntrega.Retirada, situacaoRestaurante2);

                        pedido2.AdicionarItem(item);
                        pedido2.AdicionarItem(item2);
                        pedido2.AdicionarItem(item3);

                        Console.WriteLine("Informacoes do Pedido");
                        Console.WriteLine($"Pedido numero: {pedido2.Numero}, {pedido2.HoraPedido}, {pedido2.FormaDePagamento}");

                        foreach (ItemDoPedido i in pedido2.Itens)
                        {
                            Console.WriteLine($"- {i.Quantidade}x - {i.Pedivel.Nome} - R$: {i.Subtotal}");

                        }

                        Console.WriteLine($"TOTAL R$: {pedido2.CalcularTotal()}");
                        Console.WriteLine($"{pedido2.TipoDeEntrega}");
            */

            Restaurante situacaoRestaurante2 = new Restaurante();
            situacaoRestaurante2.Aberto = true;

            CardapioDoDia cardapiododia = new CardapioDoDia(DateOnly.FromDateTime(DateTime.Now));

            cardapiododia.AdicionarOpcao(suco);
            cardapiododia.AdicionarOpcao(guarana);

            Opcao arroz = new Opcao("Arroz", Categoria.Prato);
            Opcao feijao = new Opcao("Feijao", Categoria.Prato);

            cardapiododia.AdicionarOpcao(arroz);
            cardapiododia.AdicionarOpcao(feijao);

            cardapiododia.Publicar();

            Pedido pedidodocardapio = new Pedido(004, FormadePagamento.Pix, TipoDeEntrega.Retirada, situacaoRestaurante2, cardapiododia);

            pedidodocardapio.AdicionarItem(item);
       
            marmita1.AdicionarOpcao(arroz);
            marmita1.AdicionarOpcao(feijao);

            ItemDoPedido item3 = new ItemDoPedido(marmita1, 1);

            pedidodocardapio.AdicionarItem(item3);


            Console.WriteLine("Informacoes do Pedido");
            Console.WriteLine($"Pedido numero: {pedidodocardapio.Numero}, {pedidodocardapio.HoraPedido}, {pedidodocardapio.FormaDePagamento}");

            foreach (ItemDoPedido i in pedidodocardapio.Itens)
            {
                Console.WriteLine($"- {i.Quantidade}x - {i.Pedivel.Nome} - R$: {i.Subtotal}");

            }

            Console.WriteLine($"TOTAL R$: {pedidodocardapio.CalcularTotal()}");
            Console.WriteLine($"{pedidodocardapio.TipoDeEntrega}");


            /*teste 3*/

            Opcao feijoada = new Opcao("Feijoada", Categoria.Acompanhamento);

            Marmita marmita3 = new Marmita(marmita);

            marmita3.AdicionarOpcao(feijoada);

            ItemDoPedido item4 = new ItemDoPedido(marmita3, 1);

            try
            {
                pedidodocardapio.AdicionarItem(item4);

            } catch (Exception e)
            {
                Console.WriteLine("Erro: " + e.Message);

            }

            
        }

    }
}
