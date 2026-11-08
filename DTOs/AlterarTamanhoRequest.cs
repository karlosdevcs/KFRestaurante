namespace KFRestaurante.DTOs
{
    // os limites por categoria tem endpoint proprio: PUT /tamanhos/{id}/limites
    public record AlterarTamanhoRequest(string Nome, decimal Preco, int LimitedeComponente);
}
