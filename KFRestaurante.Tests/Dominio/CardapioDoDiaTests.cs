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
