using KFRestaurante.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace KFRestaurante.DTOs

{
    public record OpcaoResponse(int Id, string Nome, Categoria Categoria, decimal? Preco, bool Esgotado)
    {
        public static OpcaoResponse De(Opcao opcao)
        {

            OpcaoResponse opcaoResponse = new OpcaoResponse(opcao.Id, opcao.Nome, opcao.Categoria, opcao.Preco, opcao.Esgotado);

            return opcaoResponse;
        }
    }

}
