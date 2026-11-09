namespace KFRestaurante.Dominio
{
    // os numeros sao o que fica gravado no banco: NUNCA mudar o numero de um papel existente.
    // O papel define so a HIERARQUIA (quem pode criar/gerenciar quem). O que cada um pode FAZER
    // vem das permissoes (ver Permissao), escolhidas por usuario.
    public enum Papel
    {
        Funcionario = 0,
        Dono = 1,
        Superuser = 2
    }
}
