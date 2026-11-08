using KFRestaurante.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.DTOs
{
    public record TamanhoResponse(int Id, string Nome, decimal Preco,  int LimitedeComponente, List<LimiteCategoriaResponse> Limites)
    {
        public static TamanhoResponse De(TamanhoMarmita tamanho)
        {
            List<LimiteCategoriaResponse> limites = tamanho.Limites.Select(limite => LimiteCategoriaResponse.De(limite)).ToList();

            TamanhoResponse tamanhoResponse = new TamanhoResponse(tamanho.Id, tamanho.Nome, tamanho.Preco, tamanho.LimitedeComponente, limites);
            return tamanhoResponse;
        }
    }
    
    
}
