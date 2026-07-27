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



            try
            {
                catalogo.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            }
            catch (Exception e)
            {
                Console.WriteLine("Erro em " + e.Message);
            }

            foreach (Opcao o in catalogo.Opcoes)
            {
                Console.WriteLine($"O catalogo contem {o.Nome} da categoria {o.Categoria}");
            }


        }

    }
}
