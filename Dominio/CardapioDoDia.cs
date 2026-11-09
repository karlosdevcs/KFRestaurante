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

		// troca a lista inteira de opcoes. So vale em rascunho: depois de publicado o cliente ja viu o cardapio
		public void SubstituirOpcoes(IEnumerable<Opcao> opcoes)
		{
			if (this.Situacao != SituacaoCardapio.Rascunho)
			{
				throw new DominioException($"Só é possível trocar as opções de um cardápio em rascunho. O cardápio de {DataPublicacao:dd/MM/yyyy} está {Situacao}.");
			}

			List<Opcao> novas = new List<Opcao>();

			foreach (Opcao opcao in opcoes)
			{
				if (novas.Any(o => o.Nome == opcao.Nome))
				{
					throw new DominioException($"A opção {opcao.Nome} já está no cardápio de {DataPublicacao:dd/MM/yyyy}.");
				}

				novas.Add(opcao);
			}

			_opcoes.Clear();
			_opcoes.AddRange(novas);
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
