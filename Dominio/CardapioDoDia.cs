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
			this.Situacao = SituacaoCardapio.Publicado;

        }

		public void AdicionarOpcao(Opcao opcao)
		{
			_opcoes.Add(opcao);
		}



    }
}
