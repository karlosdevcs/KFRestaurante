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
        public int Id { get; set; }
        public DateOnly DataPublicacao { get; set; }
		public SituacaoCardapio Situacao { get; set; }

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
					throw new InvalidOperationException("Esta opcao ja existe");

				} else
				{
                    _opcoes.Add(opcao);
                }
		
		}

		public void Publicar()
		{
			if (_opcoes.Count == 0)
				
			{
				throw new InvalidOperationException("Nao e possivel publicar um cardapio vazio");

			} else
			{
				this.Situacao = SituacaoCardapio.Publicado;
			}

		}

		public void Fechar()
		{
			if (this.Situacao == SituacaoCardapio.Rascunho)
			{
				throw new InvalidOperationException("Não é possível fechar um cardápio em rascunho.");
			}

			this.Situacao = SituacaoCardapio.Fechado; // ja fechado: continua fechado, sem erro (idempotente, igual ao Publicar)
		}

        private CardapioDoDia() { }


    }
}
