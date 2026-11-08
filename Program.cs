using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Security;
using System.Text.Json;
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

            var app = builder.Build();





            app.MapGet("/opcoes", (KFContext contexto) => {

                return contexto.Opcoes.Select(opcao => OpcaoResponse.De(opcao));


            });




            app.MapPost("/opcoes", (CriarOpcaoRequest request, KFContext contexto) =>
            {
                if (contexto.Opcoes.Any(opcao => opcao.Nome == request.Nome))
                {
                    return Results.Conflict($"A opção com o nome '{request.Nome}' já existe.");
                }

                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                contexto.SaveChanges(); //--save changes nao manda so INSERT, ele le de volta o id gerado pelo pg e escreve na inst. do change tracker. 


                return Results.Created($"/opcoes/{opcao1.Id}", OpcaoResponse.De(opcao1));

            });

            app.MapGet("/opcoes/{id}", (int id, KFContext contexto) =>
            {

                Opcao? opcao = contexto.Opcoes.Find(id);

                if (opcao == null)
                {
                    return Results.NotFound($"A opção com o id '{id}' não foi encontrada.");
                }


                return Results.Ok(OpcaoResponse.De(opcao));

            });

            app.MapGet("/tamanhos", (KFContext contexto) =>
            {
                return contexto.TamanhosMarmita.Select(tamanho => TamanhoResponse.De(tamanho));
            });

            app.MapGet("/tamanhos/{id}", (int id, KFContext contexto) =>
                {
                    TamanhoMarmita? tamanho = contexto.TamanhosMarmita.Find(id);
                    if (tamanho == null)
                    {
                        return Results.NotFound($"O tamanho com o id '{id}' não foi encontrado.");
                    }
                    return Results.Ok(TamanhoResponse.De(tamanho));
                });


            app.MapPost("/tamanhos", (CriarTamanhoRequest request, KFContext contexto) =>
            {
                if (contexto.TamanhosMarmita.Any(tamanho => tamanho.Nome == request.Nome))
                {
                    return Results.Conflict($"O tamanho com o nome '{request.Nome}' já existe.");
                }

                TamanhoMarmita tamanho1 = new TamanhoMarmita(request.Nome, request.Preco, request.LimitedeComponente);
                contexto.Add(tamanho1);
                contexto.SaveChanges();
                return Results.Created($"/tamanhos/{tamanho1.Id}", TamanhoResponse.De(tamanho1));
            });

            app.MapPost("/cardapios", (CriarCardapioRequest request, KFContext contexto) =>
            {
                List<int> idsRepetidos = request.OpcoesIds
                    .GroupBy(id => id)
                    .Where(grupo => grupo.Count() > 1)
                    .Select(grupo => grupo.Key)
                    .ToList();

                if (idsRepetidos.Count > 0)
                {
                    return Results.BadRequest($"Ids enviados mais de uma vez: {string.Join(", ", idsRepetidos)}.");
                }

                DateOnly hoje = DateOnly.FromDateTime(DateTime.Now);

                if (contexto.CardapiosDoDia.Any(cardapio => cardapio.DataPublicacao == hoje))
                {
                    return Results.Conflict($"Já existe um cardápio para a data {hoje:dd/MM/yyyy}.");
                }

                List<Opcao> opcoes = contexto.Opcoes.Where(opcao => request.OpcoesIds.Contains(opcao.Id)).ToList();

                List<int> idsInvalidos = request.OpcoesIds.Except(opcoes.Select(opcao => opcao.Id)).ToList();

                if (idsInvalidos.Count > 0)
                {
                    return Results.BadRequest($"Ids de opções não encontrados: {string.Join(", ", idsInvalidos)}.");
                }

                CardapioDoDia cardapio1 = new CardapioDoDia(hoje);

                foreach (Opcao opcao in opcoes)
                {
                    cardapio1.AdicionarOpcao(opcao);
                }

                contexto.Add(cardapio1);
                contexto.SaveChanges();

                return Results.Created($"/cardapios/{cardapio1.Id}", CardapioResponse.De(cardapio1));
            });

            app.MapGet("/cardapios/{id}", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/publicar", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                try
                {
                    cardapio.Publicar();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                contexto.SaveChanges();

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/fechar", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                try
                {
                    cardapio.Fechar();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                contexto.SaveChanges();

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapGet("/restaurante", (KFContext contexto) =>
            {
                Restaurante? restaurante = contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefault();

                if (restaurante == null)
                {
                    return Results.Ok(RestauranteResponse.De(new Restaurante())); // sem linha no banco = fechado
                }

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/restaurante/abrir", (KFContext contexto) =>
            {
                Restaurante? restaurante = contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefault();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Abrir();
                contexto.SaveChanges();

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/restaurante/fechar", (KFContext contexto) =>
            {
                Restaurante? restaurante = contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefault();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Fechar();
                contexto.SaveChanges();

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/pedidos", (CriarPedidoRequest request, KFContext contexto) =>
            {
                // 1. formato do request (sem ir ao banco)
                if (request.Itens == null || request.Itens.Count == 0)
                {
                    return Results.BadRequest("O pedido precisa ter ao menos um item.");
                }

                for (int i = 0; i < request.Itens.Count; i++)
                {
                    CriarItemDoPedidoRequest item = request.Itens[i];
                    bool ehAvulso = item.OpcaoId != null;
                    bool ehMarmita = item.TamanhoId != null;

                    if (ehAvulso == ehMarmita)
                    {
                        return Results.BadRequest($"Item {i + 1}: informe OpcaoId (item avulso) ou TamanhoId com ComponentesIds (marmita), nunca os dois nem nenhum.");
                    }

                    if (ehAvulso && item.ComponentesIds != null && item.ComponentesIds.Count > 0)
                    {
                        return Results.BadRequest($"Item {i + 1}: ComponentesIds só vale para marmita.");
                    }
                }

                // 2. tudo que ja existe vem do banco
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(c => c.Opcoes)
                    .FirstOrDefault(c => c.Id == request.CardapioId);

                if (cardapio == null)
                {
                    return Results.BadRequest($"O cardápio com o id '{request.CardapioId}' não foi encontrado.");
                }

                List<int> idsOpcoes = request.Itens
                    .Where(item => item.OpcaoId != null)
                    .Select(item => item.OpcaoId!.Value)
                    .Concat(request.Itens.SelectMany(item => item.ComponentesIds ?? new List<int>()))
                    .Distinct()
                    .ToList();

                Dictionary<int, Opcao> opcoes = contexto.Opcoes
                    .Where(opcao => idsOpcoes.Contains(opcao.Id))
                    .ToDictionary(opcao => opcao.Id);

                List<int> idsOpcoesInvalidos = idsOpcoes.Where(id => !opcoes.ContainsKey(id)).ToList();

                if (idsOpcoesInvalidos.Count > 0)
                {
                    return Results.BadRequest($"Ids de opções não encontrados: {string.Join(", ", idsOpcoesInvalidos)}.");
                }

                List<int> idsTamanhos = request.Itens
                    .Where(item => item.TamanhoId != null)
                    .Select(item => item.TamanhoId!.Value)
                    .Distinct()
                    .ToList();

                Dictionary<int, TamanhoMarmita> tamanhos = contexto.TamanhosMarmita
                    .Where(tamanho => idsTamanhos.Contains(tamanho.Id))
                    .ToDictionary(tamanho => tamanho.Id);

                List<int> idsTamanhosInvalidos = idsTamanhos.Where(id => !tamanhos.ContainsKey(id)).ToList();

                if (idsTamanhosInvalidos.Count > 0)
                {
                    return Results.BadRequest($"Ids de tamanhos não encontrados: {string.Join(", ", idsTamanhosInvalidos)}.");
                }

                // sem linha no banco = restaurante fechado, e o dominio recusa
                Restaurante restaurante = contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefault() ?? new Restaurante();

                // 3. o dominio monta o pedido e aplica as regras
                Pedido pedido;

                try
                {
                    pedido = new Pedido(request.FormaDePagamento, request.TipoDeEntrega, request.Endereco, restaurante, cardapio);

                    foreach (CriarItemDoPedidoRequest item in request.Itens)
                    {
                        if (item.OpcaoId != null)
                        {
                            pedido.AdicionarItem(opcoes[item.OpcaoId.Value], item.Quantidade);
                            continue;
                        }

                        Marmita marmita = new Marmita(tamanhos[item.TamanhoId!.Value]);

                        foreach (int componenteId in item.ComponentesIds ?? new List<int>())
                        {
                            marmita.AdicionarOpcao(opcoes[componenteId]);
                        }

                        pedido.AdicionarItem(marmita, item.Quantidade);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                contexto.Add(pedido);
                contexto.SaveChanges(); // o banco gera o Numero pela sequence e o EF le de volta

                return Results.Created($"/pedidos/{pedido.Id}", PedidoResponse.De(pedido));
            });

            app.MapGet("/pedidos/{id}", (int id, KFContext contexto) =>
            {
                Pedido? pedido = contexto.Pedidos
                    .Include(p => p.Itens)
                    .Include(p => p.CardapioDoDia)
                    .FirstOrDefault(p => p.Id == id);

                if (pedido == null)
                {
                    return Results.NotFound($"O pedido com o id '{id}' não foi encontrado.");
                }

                return Results.Ok(PedidoResponse.De(pedido));
            });


                app.UseSwagger();
                app.UseSwaggerUI();

                app.Run();
            }
    }
}
