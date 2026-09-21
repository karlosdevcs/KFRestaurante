using KFRestaurante.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace KFRestaurante.DTOs

{
    public record OpcaoResponse (int Id, string Nome, Categoria Categoria, decimal Preco, bool esgotado);
    
}
