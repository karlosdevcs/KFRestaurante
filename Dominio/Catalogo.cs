using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Catalogo
    {
        private List<Opcao> _opcoes = new List<Opcao>(); //isso deve ser uma lista porque vou ter .Add nele e posso alterar 
        //public List<Opcao> Opcoes { get; private set; } = new();
        public IEnumerable<Opcao> Opcoes => _opcoes; 

        //metodo: o que o catalogo sabe fazer? Um classe não é apenas um conjunto de dados, ela também tem comportamentos.
        //O catalogo sabe adicionar uma opcao, remover uma opcao, listar as opcoes, etc.
        //a sintaxe e similar a de um construtor, so que metodo tem nome proprio e chamamos quantas vezes quiser, o construtor so roda no NEW

        public void AdicionarOpcao(Opcao opcao)//metodo criado, a vitrine e get-only (so mostra, nao recebe) nao e possivel escrever nela, e o papel do metodo e ADICIONAR UM ITEM na lista que ja existe
            //"pega a opcao que chegou e poe ela dentro do estoque, o estoque e _opcao e colocar dentro e .add"
        {
            bool jaExiste = false;

            foreach (Opcao o in _opcoes)
            {
                if (o.Nome == opcao.Nome)
                {
                    jaExiste = true;

                }
            }
            if (jaExiste == true)
            {
                throw new InvalidOperationException("Esta opcao ja existe");
            }
            else
            {
                _opcoes.Add(opcao);
            }
        }

    }
}
