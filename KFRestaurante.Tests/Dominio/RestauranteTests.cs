using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class RestauranteTests
    {
        [Fact]
        public void Nasce_fechado_e_sem_whatsapp()
        {
            Restaurante restaurante = new Restaurante();

            Assert.False(restaurante.Aberto);
            Assert.Null(restaurante.WhatsApp);
        }

        [Theory]
        [InlineData("5531999999999", "5531999999999")]
        [InlineData("+55 (31) 99999-9999", "5531999999999")]
        [InlineData("55 31 3333-4444", "553133334444")] // fixo: 12 digitos
        public void Guarda_whatsapp_so_com_digitos(string informado, string guardado)
        {
            Restaurante restaurante = new Restaurante();

            restaurante.AlterarWhatsApp(informado);

            Assert.Equal(guardado, restaurante.WhatsApp);
        }

        [Theory]
        [InlineData("99999-9999")]        // sem pais e DDD
        [InlineData("31999999999")]       // sem pais
        [InlineData("55319999999999999")] // digitos demais
        [InlineData("")]
        public void Recusa_whatsapp_invalido(string informado)
        {
            Restaurante restaurante = new Restaurante();

            DominioException erro = Assert.Throws<DominioException>(() => restaurante.AlterarWhatsApp(informado));

            Assert.Contains("WhatsApp inválido", erro.Message);
            Assert.Null(restaurante.WhatsApp);
        }

        [Fact]
        public void Nao_abre_sem_whatsapp()
        {
            Restaurante restaurante = new Restaurante();

            DominioException erro = Assert.Throws<DominioException>(() => restaurante.Abrir());

            Assert.Contains("Cadastre o WhatsApp", erro.Message);
            Assert.False(restaurante.Aberto);
        }

        [Fact]
        public void Abre_e_fecha_com_whatsapp_cadastrado()
        {
            Restaurante restaurante = new Restaurante();
            restaurante.AlterarWhatsApp("5531999999999");

            restaurante.Abrir();
            restaurante.Abrir(); // idempotente
            Assert.True(restaurante.Aberto);

            restaurante.Fechar();
            Assert.False(restaurante.Aberto);
        }
    }
}
