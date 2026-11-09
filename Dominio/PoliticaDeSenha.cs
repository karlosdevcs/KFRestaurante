namespace KFRestaurante.Dominio
{
    public static class PoliticaDeSenha
    {
        public const int Minimo = 8;
        public const int Maximo = 128; // limite alto, mas finito: senha gigante custa caro para calcular o hash

        // as mensagens nunca repetem a senha
        public static void Validar(string? senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < Minimo)
            {
                throw new DominioException($"A senha deve ter no mínimo {Minimo} caracteres.");
            }

            if (senha.Length > Maximo)
            {
                throw new DominioException($"A senha deve ter no máximo {Maximo} caracteres.");
            }
        }
    }
}
