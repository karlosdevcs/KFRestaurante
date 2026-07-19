using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class TamanhoMarmita
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }

        public TamanhoMarmita(string nome, decimal preco)
        {
            this.Nome = nome;
            this.Preco = preco;
        }
    }
}
