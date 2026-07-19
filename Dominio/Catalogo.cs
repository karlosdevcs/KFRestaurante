using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Catalogo
    {
        public List<Opcao> Opcoes { get; private set; } = new();

        //metodo: o que o catalogo sabe fazer? Um classe não é apenas um conjunto de dados, ela também tem comportamentos.
        //O catalogo sabe adicionar uma opcao, remover uma opcao, listar as opcoes, etc.
        //a sintaxe e similar a de um construtor, so que metodo tem nome proprio e chamamos quantas vezes quiser, o construtor so roda no NEW

        public void AdicionarOpcao(Opcao opcao)//metodo criado 
        {
            Opcoes.Add(opcao);
        }

    }
}
