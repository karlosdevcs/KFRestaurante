using KFRestaurante.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KFRestaurante.DTOs
{
    public record CriarTamanhoRequest(string Nome, decimal Preco, int LimitedeComponente);
  
}
