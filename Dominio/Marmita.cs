using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Marmita: IPedivel
    {
        public int Id { get; private set; }

        public TamanhoMarmita Tamanho { get; private set; } = null!;

        private List<Opcao> _opcoes = new List<Opcao>(); //list porque tem add e remove, e onde os componentes moram 

        public IEnumerable<Opcao> Opcoes => _opcoes; //private escondida pq so enxerga a classe e ienumerable pq so da pra percorrer 

        public Marmita(TamanhoMarmita tamanho)
        {
            this.Tamanho = tamanho;
        }
         
        public void AdicionarOpcao(Opcao opcao)
        {
            if (!opcao.Categoria.EhComponenteDeMarmita())
            {
                throw new DominioException($"{opcao.Nome} é {opcao.Categoria} e não pode ser componente de marmita.");
            }

            if ( _opcoes.Count >= Tamanho.LimitedeComponente)
            {
                throw new DominioException($"A {Nome} aceita no máximo {Tamanho.LimitedeComponente} componentes. {opcao.Nome} não coube.");
            }

            // porcao dupla conta 2: cada repeticao ja esta na lista _opcoes
            int? limiteDaCategoria = Tamanho.LimiteDe(opcao.Categoria);

            if (limiteDaCategoria != null && _opcoes.Count(o => o.Categoria == opcao.Categoria) >= limiteDaCategoria)
            {
                string dica = _opcoes.Contains(opcao) ? " Porção dupla conta como 2." : "";

                throw new DominioException($"A {Nome} aceita no máximo {limiteDaCategoria} de {opcao.Categoria}.{dica}");
            }

            _opcoes.Add(opcao);

       
        }

        public decimal Preco => Tamanho.Preco;

        public string Nome => $"Marmita {Tamanho.Nome}";

        private Marmita() { }
    }
}
