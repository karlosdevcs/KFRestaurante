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

		// UTC. Quando as OPCOES do cardapio mudaram pela ultima vez (criacao, inclusao ou troca): o painel mostra "atualizado as HH:mm"
		public DateTime AtualizadoEm { get; private set; }

		private List<Opcao> _opcoes = new List<Opcao>();
		public IEnumerable<Opcao> Opcoes => _opcoes;

		public CardapioDoDia(DateOnly dataPublicacao)
		{
			this.DataPublicacao = dataPublicacao;
			this.Situacao = SituacaoCardapio.Rascunho;
			this.AtualizadoEm = DateTime.UtcNow;

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
                    this.AtualizadoEm = DateTime.UtcNow;
                }
		
		}

		// troca a lista de opcoes.
		//  - Rascunho: troca livre (o cliente ainda nao viu nada).
		//  - Publicado: so ACRESCENTA (o prato novo que o dono resolveu vender hoje). Tirar um item e pelo "esgotado",
		//    porque o cliente ja viu o cardapio e pode ter o item no carrinho.
		//  - Fechado: recusa; reabra antes.
		public void SubstituirOpcoes(IEnumerable<Opcao> opcoes)
		{
			if (this.Situacao == SituacaoCardapio.Fechado)
			{
				throw new DominioException($"O cardápio de {DataPublicacao:dd/MM/yyyy} está fechado. Reabra o cardápio para mudar as opções.");
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

			if (this.Situacao == SituacaoCardapio.Publicado)
			{
				List<string> removidas = _opcoes.Where(atual => !novas.Any(nova => nova.Nome == atual.Nome)).Select(atual => atual.Nome).ToList();

				if (removidas.Count > 0)
				{
					throw new DominioException($"Um cardápio publicado só aceita opções novas. Para tirar {string.Join(", ", removidas)} hoje, marque como esgotado.");
				}
			}

			_opcoes.Clear();
			_opcoes.AddRange(novas);
			this.AtualizadoEm = DateTime.UtcNow;
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
