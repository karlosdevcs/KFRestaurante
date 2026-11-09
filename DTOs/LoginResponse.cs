namespace KFRestaurante.DTOs
{
    public record LoginResponse(string Token, DateTime ExpiraEm, UsuarioResponse Usuario);
}
