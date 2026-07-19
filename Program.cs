using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Opcao> opcoes = new List<Opcao>();

            opcoes.Add(new Opcao("Arroz", Categoria.Acompanhamento));
            opcoes.Add(new Opcao("Batata", Categoria.Prato));

            //Console.WriteLine(opcoes[0]); //o writeline recebeu um objeto e n'ao sabe imprimir uma opcao, ele pede ao proprio objeto descreva-se. Toda classe em C# herda uma autodescricao padrao, e ela diz apenas o nome completo do tipo (namespace+ classe)

            foreach (Opcao o in opcoes)
            {
                Console.WriteLine($"O pedido contem {o.Nome} que e um {o.Categoria}");

            }

        }

    }
}
