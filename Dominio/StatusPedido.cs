namespace KFRestaurante.Dominio
{
    // os numeros sao o que fica gravado no banco: NUNCA mudar o numero de um status existente.
    public enum StatusPedido
    {
        Gerado = 0,
        Enviado = 1
    }
}
