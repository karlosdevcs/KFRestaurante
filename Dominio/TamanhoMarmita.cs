using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class TamanhoMarmita
    {

        public int Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public decimal Preco { get; private set; }

        public int LimitedeComponente { get; private set; }

        // limites por categoria definidos pelo dono. Categoria sem limite fica livre (respeitando so o LimitedeComponente)
        private List<LimiteCategoria> _limites = new List<LimiteCategoria>();
        public IEnumerable<LimiteCategoria> Limites => _limites;

        public TamanhoMarmita(string nome, decimal preco, int limitedecomponente)
        {
            Validar(preco, limitedecomponente);

            this.Nome = nome;
            this.Preco = preco;
            this.LimitedeComponente = limitedecomponente;
        }

        // vale dali em diante: pedidos ja feitos guardam a foto do preco
        public void Alterar(string nome, decimal preco, int limitedecomponente)
        {
            Validar(preco, limitedecomponente);

            this.Nome = nome;
            this.Preco = preco;
            this.LimitedeComponente = limitedecomponente;
        }

        private static void Validar(decimal preco, int limitedecomponente)
        {
            if (preco < 0)
            {
                throw new DominioException($"O preço do tamanho não pode ser negativo. Valor recebido: {preco}.");
            }

            if (limitedecomponente < 0)
            {
                throw new DominioException($"O limite de componentes não pode ser negativo. Valor recebido: {limitedecomponente}.");
            }
        }

        // substitui todos os limites de uma vez. Vale dali em diante: pedidos ja feitos guardam a foto
        public void DefinirLimites(IEnumerable<LimiteCategoria> limites)
        {
            List<LimiteCategoria> novos = limites.ToList();

            List<Categoria> repetidas = novos
                .GroupBy(limite => limite.Categoria)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToList();

            if (repetidas.Count > 0)
            {
                throw new DominioException($"Categoria informada mais de uma vez: {string.Join(", ", repetidas)}.");
            }

            _limites.Clear();
            _limites.AddRange(novos);
        }

        // null = sem limite para essa categoria
        public int? LimiteDe(Categoria categoria)
        {
            return _limites.FirstOrDefault(limite => limite.Categoria == categoria)?.Maximo;
        }

        private TamanhoMarmita() { }
    }
}
