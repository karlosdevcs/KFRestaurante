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

            var connectionString = builder.Configuration.GetConnectionString("KFRestaurante");
            builder.Services.AddDbContext<KFContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ManipuladorDeExcecoes>();

            var app = builder.Build();

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
