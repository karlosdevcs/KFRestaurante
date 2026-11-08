namespace KFRestaurante.Dominio
{
    // regra de negocio violada. O ManipuladorDeExcecoes transforma em 400 com a mensagem.
    public class DominioException : Exception
    {
        public DominioException(string mensagem) : base(mensagem)
        {
        }
    }
}
