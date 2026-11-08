using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KFRestaurante.Dominio
{
    // os numeros sao o que fica gravado no banco: NUNCA mudar o numero de uma categoria existente.
    // Carne ocupa o 0 que era do antigo Prato (as opcoes que eram Prato viraram Carne).
    public enum Categoria
    {
        // componentes de marmita
        Carne = 0,
        Acompanhamento = 1,
        Guarnicao = 4,
        Salada = 5,

        // so vendidos avulsos
        Bebida = 2,
        Sobremesa = 3
    }

    public static class CategoriaExtensions
    {
        // uso: opcao.Categoria.EhComponenteDeMarmita()
        public static bool EhComponenteDeMarmita(this Categoria categoria)
        {
            return categoria != Categoria.Bebida && categoria != Categoria.Sobremesa;
        }
    }
}
