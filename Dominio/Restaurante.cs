using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Restaurante
    {
        public int Id { get; set; }
        public bool Aberto { get; set; }

        public Restaurante()
        {
            this.Aberto = false;
        }

       


    }
}
