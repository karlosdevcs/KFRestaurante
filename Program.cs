using KFRestaurante.Dominio;
using System.Linq.Expressions;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

      /*      Opcao coca = new Opcao("Coca-cola", Categoria.Bebida, 5);
            ItemDoPedido item = new ItemDoPedido(coca, 2);

            Console.WriteLine($"Preco {item.PrecoUnitario}");
            Console.WriteLine($"Quantidade {item.Quantidade}");
            Console.WriteLine($"Subtotal {item.Subtotal}");

            coca.Preco = 8;
            Console.WriteLine($"Preco da coca agora: {coca.Preco}");
            Console.WriteLine($"Preco unitario do item (deve continuar 5): {item.PrecoUnitario}");
            Console.WriteLine($"Subtotal do item (deve continuar 10): {item.Subtotal}");*/

            TamanhoMarmita tamanhomarmita = new TamanhoMarmita("G", 25);
            Marmita marmita1 = new Marmita(tamanhomarmita);

            Console.WriteLine($"O preco da marmita e {marmita1.PrecoMarmita}");

            Opcao feijaotropeiro = new Opcao("Feijao tropeiro", Categoria.Prato);

            marmita1.AdicionarOpcao(feijaotropeiro);  
            
            foreach (Opcao o in marmita1.Opcoes)
            {
                Console.WriteLine($"O nome escolhido foi {o.Nome}, {o.Categoria}");
                
            }

            Console.WriteLine($"Preco da marmita e {marmita1.PrecoMarmita}");
        }

    }
}
