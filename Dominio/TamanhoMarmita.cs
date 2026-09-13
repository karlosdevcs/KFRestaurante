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

        public int LimitedeComponente { get; set; }

        public TamanhoMarmita(string nome, decimal preco, int limitedecomponente)
        {
            this.Nome = nome;
            this.Preco = preco;
            this.LimitedeComponente = limitedecomponente;
        }
    }
}
