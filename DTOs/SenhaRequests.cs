namespace KFRestaurante.DTOs
{
    // a propria pessoa troca a sua senha (precisa da atual)
    public record TrocarSenhaRequest(string SenhaAtual, string NovaSenha);

    // um superior redefine a senha de quem ele gerencia (esqueceu a senha)
    public record RedefinirSenhaRequest(string NovaSenha);
}
