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

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ManipuladorDeExcecoes>();

            var app = builder.Build();

            app.UseExceptionHandler(); // toda excecao nao tratada passa pelo ManipuladorDeExcecoes





            app.MapGet("/opcoes", async (KFContext contexto) => {

                return await contexto.Opcoes.Select(opcao => OpcaoResponse.De(opcao)).ToListAsync();


            });




            app.MapPost("/opcoes", async (CriarOpcaoRequest request, KFContext contexto) =>
            {
                if (await contexto.Opcoes.AnyAsync(opcao => opcao.Nome == request.Nome))
                {
                    return Results.Problem(detail: $"A opção com o nome '{request.Nome}' já existe.", statusCode: StatusCodes.Status409Conflict);
                }

                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                await contexto.SaveChangesAsync(); //--save changes nao manda so INSERT, ele le de volta o id gerado pelo pg e escreve na inst. do change tracker. 


                return Results.Created($"/opcoes/{opcao1.Id}", OpcaoResponse.De(opcao1));

            });

            app.MapGet("/opcoes/{id}", async (int id, KFContext contexto) =>
            {

                Opcao? opcao = await contexto.Opcoes.FindAsync(id);

                if (opcao == null)
                {
                    return Results.Problem(detail: $"A opção com o id '{id}' não foi encontrada.", statusCode: StatusCodes.Status404NotFound);
                }


                return Results.Ok(OpcaoResponse.De(opcao));

            });

            app.MapPatch("/opcoes/{id}/esgotado", async (int id, AlterarEsgotadoRequest request, KFContext contexto) =>
            {
                Opcao? opcao = await contexto.Opcoes.FindAsync(id);

                if (opcao == null)
                {
                    return Results.Problem(detail: $"A opção com o id '{id}' não foi encontrada.", statusCode: StatusCodes.Status404NotFound);
                }

                opcao.AlterarEsgotado(request.Esgotado);
                await contexto.SaveChangesAsync();

                return Results.Ok(OpcaoResponse.De(opcao));
            });

            app.MapGet("/tamanhos", async (KFContext contexto) =>
            {
                return await contexto.TamanhosMarmita.Select(tamanho => TamanhoResponse.De(tamanho)).ToListAsync();
            });

            app.MapGet("/tamanhos/{id}", async (int id, KFContext contexto) =>
                {
                    TamanhoMarmita? tamanho = await contexto.TamanhosMarmita.FindAsync(id);
                    if (tamanho == null)
                    {
                        return Results.Problem(detail: $"O tamanho com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                    }
                    return Results.Ok(TamanhoResponse.De(tamanho));
                });


            app.MapPost("/tamanhos", async (CriarTamanhoRequest request, KFContext contexto) =>
            {
                if (await contexto.TamanhosMarmita.AnyAsync(tamanho => tamanho.Nome == request.Nome))
                {
                    return Results.Problem(detail: $"O tamanho com o nome '{request.Nome}' já existe.", statusCode: StatusCodes.Status409Conflict);
                }

                TamanhoMarmita tamanho1 = new TamanhoMarmita(request.Nome, request.Preco, request.LimitedeComponente);
                contexto.Add(tamanho1);
                await contexto.SaveChangesAsync();
                return Results.Created($"/tamanhos/{tamanho1.Id}", TamanhoResponse.De(tamanho1));
            });

            app.MapPost("/cardapios", async (CriarCardapioRequest request, KFContext contexto) =>
            {
                List<int> idsRepetidos = request.OpcoesIds
                    .GroupBy(id => id)
                    .Where(grupo => grupo.Count() > 1)
                    .Select(grupo => grupo.Key)
                    .ToList();

                if (idsRepetidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids enviados mais de uma vez: {string.Join(", ", idsRepetidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                DateOnly hoje = DateOnly.FromDateTime(DateTime.Now);

                if (await contexto.CardapiosDoDia.AnyAsync(cardapio => cardapio.DataPublicacao == hoje))
                {
                    return Results.Problem(detail: $"Já existe um cardápio para a data {hoje:dd/MM/yyyy}.", statusCode: StatusCodes.Status409Conflict);
                }

                List<Opcao> opcoes = await contexto.Opcoes.Where(opcao => request.OpcoesIds.Contains(opcao.Id)).ToListAsync();

                List<int> idsInvalidos = request.OpcoesIds.Except(opcoes.Select(opcao => opcao.Id)).ToList();

                if (idsInvalidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids de opções não encontrados: {string.Join(", ", idsInvalidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                CardapioDoDia cardapio1 = new CardapioDoDia(hoje);

                foreach (Opcao opcao in opcoes)
                {
                    cardapio1.AdicionarOpcao(opcao);
                }

                contexto.Add(cardapio1);
                await contexto.SaveChangesAsync();

                return Results.Created($"/cardapios/{cardapio1.Id}", CardapioResponse.De(cardapio1));
            });

            app.MapGet("/cardapios/{id}", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/publicar", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                cardapio.Publicar();
                await contexto.SaveChangesAsync();

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/fechar", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                cardapio.Fechar();
                await contexto.SaveChangesAsync();

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapGet("/restaurante", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    return Results.Ok(RestauranteResponse.De(new Restaurante())); // sem linha no banco = fechado
                }

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/restaurante/abrir", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Abrir();
                await contexto.SaveChangesAsync();

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/restaurante/fechar", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Fechar();
                await contexto.SaveChangesAsync();

                return Results.Ok(RestauranteResponse.De(restaurante));
            });

            app.MapPost("/pedidos", async (CriarPedidoRequest request, KFContext contexto) =>
            {
                // 1. formato do request (sem ir ao banco)
                if (request.Itens == null || request.Itens.Count == 0)
                {
                    return Results.Problem(detail: "O pedido precisa ter ao menos um item.", statusCode: StatusCodes.Status400BadRequest);
                }

                for (int i = 0; i < request.Itens.Count; i++)
                {
                    CriarItemDoPedidoRequest item = request.Itens[i];
                    bool ehAvulso = item.OpcaoId != null;
                    bool ehMarmita = item.TamanhoId != null;

                    if (ehAvulso == ehMarmita)
                    {
                        return Results.Problem(detail: $"Item {i + 1}: informe OpcaoId (item avulso) ou TamanhoId com ComponentesIds (marmita), nunca os dois nem nenhum.", statusCode: StatusCodes.Status400BadRequest);
                    }

                    if (ehAvulso && item.ComponentesIds != null && item.ComponentesIds.Count > 0)
                    {
                        return Results.Problem(detail: $"Item {i + 1}: ComponentesIds só vale para marmita.", statusCode: StatusCodes.Status400BadRequest);
                    }
                }

                // 2. tudo que ja existe vem do banco
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(c => c.Opcoes)
                    .FirstOrDefaultAsync(c => c.Id == request.CardapioId);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{request.CardapioId}' não foi encontrado.", statusCode: StatusCodes.Status400BadRequest);
                }

                List<int> idsOpcoes = request.Itens
                    .Where(item => item.OpcaoId != null)
                    .Select(item => item.OpcaoId!.Value)
                    .Concat(request.Itens.SelectMany(item => item.ComponentesIds ?? new List<int>()))
                    .Distinct()
                    .ToList();

                Dictionary<int, Opcao> opcoes = await contexto.Opcoes
                    .Where(opcao => idsOpcoes.Contains(opcao.Id))
                    .ToDictionaryAsync(opcao => opcao.Id);

                List<int> idsOpcoesInvalidos = idsOpcoes.Where(id => !opcoes.ContainsKey(id)).ToList();

                if (idsOpcoesInvalidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids de opções não encontrados: {string.Join(", ", idsOpcoesInvalidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                List<int> idsTamanhos = request.Itens
                    .Where(item => item.TamanhoId != null)
                    .Select(item => item.TamanhoId!.Value)
                    .Distinct()
                    .ToList();

                Dictionary<int, TamanhoMarmita> tamanhos = await contexto.TamanhosMarmita
                    .Where(tamanho => idsTamanhos.Contains(tamanho.Id))
                    .ToDictionaryAsync(tamanho => tamanho.Id);

                List<int> idsTamanhosInvalidos = idsTamanhos.Where(id => !tamanhos.ContainsKey(id)).ToList();

                if (idsTamanhosInvalidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids de tamanhos não encontrados: {string.Join(", ", idsTamanhosInvalidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                // sem linha no banco = restaurante fechado, e o dominio recusa
                Restaurante restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync() ?? new Restaurante();

                // 3. o dominio monta o pedido e aplica as regras (DominioException vira 400 no ManipuladorDeExcecoes)
                Pedido pedido = new Pedido(request.FormaDePagamento, request.TipoDeEntrega, request.Endereco, restaurante, cardapio);

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

                contexto.Add(pedido);
                await contexto.SaveChangesAsync(); // o banco gera o Numero pela sequence e o EF le de volta

                return Results.Created($"/pedidos/{pedido.Id}", PedidoResponse.De(pedido));
            });

            app.MapGet("/pedidos/{id}", async (int id, KFContext contexto) =>
            {
                Pedido? pedido = await contexto.Pedidos
                    .Include(p => p.Itens)
                    .Include(p => p.CardapioDoDia)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return Results.Problem(detail: $"O pedido com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                return Results.Ok(PedidoResponse.De(pedido));
            });


                app.UseSwagger();
                app.UseSwaggerUI();

                app.Run();
            }
    }
}
