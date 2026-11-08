using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Restaurante
    {
        public int Id { get; private set; }
        public bool Aberto { get; private set; }

        public Restaurante()
        {
            this.Aberto = false;
        }

        public void Abrir()
        {
            this.Aberto = true; // ja aberto: continua aberto, sem erro (idempotente)
        }

        public void Fechar()
        {
            this.Aberto = false; // ja fechado: continua fechado, sem erro (idempotente)
        }

    }
}
