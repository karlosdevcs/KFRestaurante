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
    }
}
