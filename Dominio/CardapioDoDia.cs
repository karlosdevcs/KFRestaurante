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
				} else
				{
                    _opcoes.Add(opcao);
                }
			
			//_opcoes.Add(opcao);
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



    }
}
