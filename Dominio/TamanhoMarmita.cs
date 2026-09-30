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

        public TamanhoMarmita(string nome, decimal preco, int limitedecomponente)
        {
            this.Nome = nome;
            this.Preco = preco;
            this.LimitedeComponente = limitedecomponente;
        }

        private TamanhoMarmita() { }
    }
}
