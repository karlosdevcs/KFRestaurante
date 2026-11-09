namespace KFRestaurante.Dominio
{
    // Cada permissao e um bit. O conjunto de permissoes de um usuario e um unico inteiro no banco.
    // Os valores ficam gravados: NUNCA renumerar uma permissao existente, so acrescentar (64, 128...).
    [Flags]
    public enum Permissao
    {
        Nenhuma = 0,
        VerPedidos = 1,
        EsgotarOpcoes = 2,
        AbrirFecharLoja = 4,
        GerenciarCardapio = 8,
        GerenciarCadastros = 16,
        AlterarWhatsApp = 32
    }

    public static class PermissaoExtensions
    {
        // todas as permissoes que existem (sem "Nenhuma")
        public static IReadOnlyList<Permissao> Individuais { get; } =
            Enum.GetValues<Permissao>().Where(p => p != Permissao.Nenhuma).ToList();

        public static Permissao Todas { get; } = Individuais.Aggregate(Permissao.Nenhuma, (soma, p) => soma | p);

        // transforma a lista recebida da API em um conjunto, recusando valor desconhecido ou combinado
        public static Permissao Combinar(IEnumerable<Permissao>? permissoes)
        {
            Permissao conjunto = Permissao.Nenhuma;

            foreach (Permissao permissao in permissoes ?? Enumerable.Empty<Permissao>())
            {
                if (!Individuais.Contains(permissao))
                {
                    throw new DominioException($"Permissão inválida: '{permissao}'.");
                }

                conjunto |= permissao;
            }

            return conjunto;
        }

        // o inverso: conjunto -> lista de permissoes individuais (para mostrar na API)
        public static List<Permissao> Separar(this Permissao conjunto)
        {
            return Individuais.Where(p => conjunto.HasFlag(p)).ToList();
        }
    }
}
