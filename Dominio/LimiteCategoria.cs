namespace KFRestaurante.Dominio
{
    // "ate N componentes desta categoria" dentro de um tamanho de marmita.
    // Nao existe sozinho: faz parte do TamanhoMarmita (no EF e uma owned entity).
    public class LimiteCategoria
    {
        public Categoria Categoria { get; private set; }
        public int Maximo { get; private set; }

        public LimiteCategoria(Categoria categoria, int maximo)
        {
            if (!categoria.EhComponenteDeMarmita())
            {
                throw new DominioException($"{categoria} não é componente de marmita e não pode ter limite.");
            }

            if (maximo < 0)
            {
                throw new DominioException($"O limite de {categoria} não pode ser negativo. Valor recebido: {maximo}.");
            }

            this.Categoria = categoria;
            this.Maximo = maximo;
        }

        private LimiteCategoria() { }
    }
}
