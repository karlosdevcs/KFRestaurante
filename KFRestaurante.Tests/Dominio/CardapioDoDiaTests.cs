using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class CardapioDoDiaTests
    {
        private static readonly DateOnly Hoje = new DateOnly(2026, 11, 8);

        [Fact]
        public void Nasce_em_rascunho()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);

            Assert.Equal(SituacaoCardapio.Rascunho, cardapio.Situacao);
            Assert.Equal(Hoje, cardapio.DataPublicacao);
        }

        [Fact]
        public void Recusa_opcao_repetida()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            DominioException erro = Assert.Throws<DominioException>(() => cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento)));

            Assert.Contains("Arroz", erro.Message);
        }

        [Fact]
        public void Substitui_as_opcoes_em_rascunho()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            cardapio.SubstituirOpcoes(new[] { new Opcao("Feijão", Categoria.Acompanhamento), new Opcao("Bife", Categoria.Carne) });

            Assert.Equal(new[] { "Feijão", "Bife" }, cardapio.Opcoes.Select(o => o.Nome).ToArray());
        }

        [Fact]
        public void Recusa_substituir_opcoes_de_cardapio_publicado_ou_fechado()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));
            cardapio.Publicar();

            Assert.Throws<DominioException>(() => cardapio.SubstituirOpcoes(new[] { new Opcao("Bife", Categoria.Carne) }));

            cardapio.Fechar();
            Assert.Throws<DominioException>(() => cardapio.SubstituirOpcoes(new[] { new Opcao("Bife", Categoria.Carne) }));
            Assert.Equal("Arroz", cardapio.Opcoes.Single().Nome); // nada mudou
        }

        [Fact]
        public void Recusa_nome_repetido_ao_substituir_e_mantem_as_opcoes_antigas()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            Assert.Throws<DominioException>(() => cardapio.SubstituirOpcoes(new[] { new Opcao("Bife", Categoria.Carne), new Opcao("Bife", Categoria.Carne) }));

            Assert.Equal("Arroz", cardapio.Opcoes.Single().Nome);
        }

        [Fact]
        public void Recusa_publicar_vazio()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);

            Assert.Throws<DominioException>(() => cardapio.Publicar());
            Assert.Equal(SituacaoCardapio.Rascunho, cardapio.Situacao);
        }

        [Fact]
        public void Recusa_fechar_rascunho()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);

            Assert.Throws<DominioException>(() => cardapio.Fechar());
        }

        [Fact]
        public void Publica_fecha_e_reabre()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            cardapio.Publicar();
            Assert.Equal(SituacaoCardapio.Publicado, cardapio.Situacao);

            cardapio.Fechar();
            Assert.Equal(SituacaoCardapio.Fechado, cardapio.Situacao);

            cardapio.Publicar();
            Assert.Equal(SituacaoCardapio.Publicado, cardapio.Situacao);
        }

        [Fact]
        public void Publicar_e_fechar_sao_idempotentes()
        {
            CardapioDoDia cardapio = new CardapioDoDia(Hoje);
            cardapio.AdicionarOpcao(new Opcao("Arroz", Categoria.Acompanhamento));

            cardapio.Publicar();
            cardapio.Publicar();
            Assert.Equal(SituacaoCardapio.Publicado, cardapio.Situacao);

            cardapio.Fechar();
            cardapio.Fechar();
            Assert.Equal(SituacaoCardapio.Fechado, cardapio.Situacao);
        }
    }
}
