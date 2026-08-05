using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Marmita
    {
        public TamanhoMarmita Tamanho {get; private set;}

        private List<Opcao> _opcoes = new List<Opcao>(); //list porque tem add e remove, e onde os componentes moram 

        public IEnumerable<Opcao> Opcoes => _opcoes; //private escondida pq so enxerga a classe e ienumerable pq so da pra percorrer 

        public Marmita(TamanhoMarmita tamanho)
        {
            this.Tamanho = tamanho;
        }
         
        public void AdicionarOpcao(Opcao opcao)
        {
            _opcoes.Add(opcao); 
        }

        public decimal PrecoMarmita => Tamanho.Preco; 
    }
}
