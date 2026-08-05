using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

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
