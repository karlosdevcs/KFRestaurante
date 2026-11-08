using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace KFRestaurante.Dominio
{

	public class CardapioDoDia
	{
        public int Id { get; private set; }
        public DateOnly DataPublicacao { get; private set; }
		public SituacaoCardapio Situacao { get; private set; }

		private List<Opcao> _opcoes = new List<Opcao>();
		public IEnumerable<Opcao> Opcoes => _opcoes;

		public CardapioDoDia(DateOnly dataPublicacao)
		{
			this.DataPublicacao = dataPublicacao;
			this.Situacao = SituacaoCardapio.Rascunho;

        }

		public void AdicionarOpcao(Opcao opcao)
		{
			bool jaExiste = _opcoes.Any(o => o.Nome == opcao.Nome);

				if (jaExiste == true)
				{
					throw new DominioException($"A opção {opcao.Nome} já está no cardápio de {DataPublicacao:dd/MM/yyyy}.");

				} else
				{
                    _opcoes.Add(opcao);
                }
		
		}

		public void Publicar()
		{
			if (_opcoes.Count == 0)
				
			{
				throw new DominioException($"Não é possível publicar o cardápio de {DataPublicacao:dd/MM/yyyy}: ele não tem nenhuma opção.");

			} else
			{
				this.Situacao = SituacaoCardapio.Publicado;
			}

		}

		public void Fechar()
		{
			if (this.Situacao == SituacaoCardapio.Rascunho)
			{
				throw new DominioException("Não é possível fechar um cardápio em rascunho.");
			}

			this.Situacao = SituacaoCardapio.Fechado; // ja fechado: continua fechado, sem erro (idempotente, igual ao Publicar)
		}

        private CardapioDoDia() { }


    }
}
