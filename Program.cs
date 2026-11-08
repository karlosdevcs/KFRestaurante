using KFRestaurante.Endpoints;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            });

            // o Swagger (Swashbuckle) le as opcoes de JSON do MVC, nao as do minimal API acima.
            // Sem isto ele documenta os enums como numeros (0, 1, 2...) embora a API responda com nomes
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var connectionString = builder.Configuration.GetConnectionString("KFRestaurante");
            builder.Services.AddDbContext<KFContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ManipuladorDeExcecoes>();

            // o front roda em outro endereco (outra porta ou dominio): sem CORS o navegador bloqueia as chamadas.
            // Os enderecos ficam no appsettings para incluir o dominio do site publicado sem recompilar
            string[] origensPermitidas = builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy
                    .WithOrigins(origensPermitidas)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            var app = builder.Build();

            // antes do tratamento de erros: assim as respostas de erro (400, 404...) tambem levam os cabecalhos de CORS
            // e o front consegue ler a mensagem do ProblemDetails
            app.UseCors();

            app.UseExceptionHandler(); // toda excecao nao tratada passa pelo ManipuladorDeExcecoes

            app.MapOpcoes();
            app.MapTamanhos();
            app.MapCardapios();
            app.MapRestaurante();
            app.MapPedidos();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}
