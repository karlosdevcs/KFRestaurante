using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

            Catalogo catalogo = new Catalogo();

            catalogo.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));
            catalogo.AdicionarOpcao(new Opcao("Batata", Categoria.Prato));

            foreach (Opcao o in catalogo.Opcoes)
            {
                Console.WriteLine($"O cardapio contem {o.Nome} que é um {o.Categoria}");
            }

        }

    }
}
