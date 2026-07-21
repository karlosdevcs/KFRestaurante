using KFRestaurante.Dominio;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {
       
            CardapioDoDia vazio = new CardapioDoDia(DateOnly.FromDateTime(DateTime.Now));

            try
            {
                vazio.Publicar();
            } 
            catch (Exception e)
            {
                Console.WriteLine("Erro em " + e.Message);
            }

            CardapioDoDia comOpcao = new CardapioDoDia(DateOnly.FromDateTime(DateTime.Now));
            Opcao arroz = new Opcao("Arroz", Categoria.Acompanhamento);

            comOpcao.AdicionarOpcao(arroz);

            comOpcao.Publicar(); //OBJETO.METODO
            Console.WriteLine(comOpcao.Situacao);


        }

    }
}
