using KFRestaurante.Servicos;

namespace KFRestaurante.Tests.Servicos
{
    public class HorarioBrasiliaTests
    {
        [Fact]
        public void Noite_no_Brasil_ainda_e_o_mesmo_dia_mesmo_que_ja_seja_amanha_em_UTC()
        {
            // 08/11 23:30 em Brasilia (UTC-3) = 09/11 02:30 em UTC
            DateTime utc = new DateTime(2026, 11, 9, 2, 30, 0, DateTimeKind.Utc);

            DateTime brasilia = HorarioBrasilia.DeUtc(utc);

            Assert.Equal(new DateTime(2026, 11, 8, 23, 30, 0), brasilia);
        }

        [Fact]
        public void Hoje_e_a_data_de_Brasilia()
        {
            Assert.Equal(DateOnly.FromDateTime(HorarioBrasilia.Agora()), HorarioBrasilia.Hoje());
        }

        [Fact]
        public void Intervalo_do_dia_cobre_00h_a_23h59_em_brasilia()
        {
            // 08/11 00:00 em Brasilia (UTC-3) = 08/11 03:00 UTC; o dia termina onde comeca o proximo (09/11 03:00 UTC)
            (DateTime inicioUtc, DateTime fimUtc) = HorarioBrasilia.IntervaloUtcDoDia(new DateOnly(2026, 11, 8));

            Assert.Equal(new DateTime(2026, 11, 8, 3, 0, 0), inicioUtc);
            Assert.Equal(new DateTime(2026, 11, 9, 3, 0, 0), fimUtc);
        }

        [Fact]
        public void Pedido_de_23h30_em_brasilia_cai_no_intervalo_do_mesmo_dia()
        {
            // 08/11 23:30 em Brasilia = 09/11 02:30 UTC
            DateTime horaPedidoUtc = new DateTime(2026, 11, 9, 2, 30, 0);

            (DateTime inicioUtc, DateTime fimUtc) = HorarioBrasilia.IntervaloUtcDoDia(new DateOnly(2026, 11, 8));

            Assert.InRange(horaPedidoUtc, inicioUtc, fimUtc.AddTicks(-1));
        }
    }
}
