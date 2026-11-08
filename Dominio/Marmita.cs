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
            if (opcao.Categoria == Categoria.Bebida || opcao.Categoria == Categoria.Sobremesa)
            {
                throw new DominioException($"{opcao.Nome} é {opcao.Categoria} e não pode ser componente de marmita.");
            }

            if ( _opcoes.Count >= Tamanho.LimitedeComponente)
            {
                throw new DominioException("O limite de itens excedeu");
            } else
            {
                _opcoes.Add(opcao);
            }

       
        }

        public decimal Preco => Tamanho.Preco;

        public string Nome => $"Marmita {Tamanho.Nome}";

        private Marmita() { }
    }
}
