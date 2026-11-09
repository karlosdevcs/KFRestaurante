namespace KFRestaurante.Servicos
{
    // O restaurante funciona no horario de Brasilia, mas o servidor pode estar em qualquer fuso
    // (em nuvem, quase sempre UTC). Sem isto, depois das 21h o "hoje" do servidor ja seria amanha.
    public static class HorarioBrasilia
    {
        // "America/Sao_Paulo" existe no Windows (.NET 6+) e no Linux
        private static readonly TimeZoneInfo Fuso = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public static DateOnly Hoje()
        {
            return DateOnly.FromDateTime(Agora());
        }

        public static DateTime Agora()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso);
        }

        // o banco guarda em UTC; para exibir, converte
        public static DateTime DeUtc(DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Fuso);
        }

        // o intervalo em UTC que corresponde a um dia inteiro (00:00 a 23:59:59...) em Brasilia.
        // Usado para filtrar no banco (que guarda em UTC) "pedidos feitos no dia X"
        public static (DateTime InicioUtc, DateTime FimUtc) IntervaloUtcDoDia(DateOnly dia)
        {
            DateTime inicioLocal = dia.ToDateTime(TimeOnly.MinValue);
            DateTime fimLocal = dia.AddDays(1).ToDateTime(TimeOnly.MinValue);

            DateTime inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(inicioLocal, DateTimeKind.Unspecified), Fuso);
            DateTime fimUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(fimLocal, DateTimeKind.Unspecified), Fuso);

            return (inicioUtc, fimUtc);
        }
    }
}
