using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {

            Catalogo catalogo = new Catalogo();

            //catalogo.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));
            catalogo.AdicionarOpcao(new Opcao("Batata", Categoria.Prato));

            foreach (Opcao o in catalogo.Opcoes)
            {
                Console.WriteLine($"O cardapio contem {o.Nome} que é um {o.Categoria}");
            }

            CardapioDoDia cardapioDoDia = new CardapioDoDia(DateOnly.FromDateTime(DateTime.Now));

            //criando o arroz e adicionando ao catalogo e ao cardapio do dia (o catalogo é o repositório de todas as opções, e o cardapio do dia é o que será servido no dia)
            Opcao arroz = new Opcao("Arroz", Categoria.Acompanhamento);//cria o arroz aqui 
            catalogo.AdicionarOpcao(arroz);//adiciona o arroz ao catalogo
            cardapioDoDia.AdicionarOpcao(arroz);//adiciona o arroz ao cardapio do dia


            foreach (Opcao o in cardapioDoDia.Opcoes)
            {
                Console.WriteLine($"O cardapio do dia contem {o.Nome} que é um {o.Categoria}");
               
            }
            
            Console.WriteLine($"A situacao do cardapio do dia é: {cardapioDoDia.Situacao}");

        }

    }
}
