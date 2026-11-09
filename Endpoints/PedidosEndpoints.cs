using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Servicos;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class PedidosEndpoints
    {
        public static void MapPedidos(this IEndpointRouteBuilder app)
        {
            // painel da loja: lista os pedidos de um dia (hoje por padrao), do mais recente para o mais antigo
            app.MapGet("/pedidos", async (DateOnly? data, StatusPedido? status, KFContext contexto) =>
            {
                DateOnly dia = data ?? HorarioBrasilia.Hoje();
                (DateTime inicioUtc, DateTime fimUtc) = HorarioBrasilia.IntervaloUtcDoDia(dia);

                IQueryable<Pedido> consulta = contexto.Pedidos
                    .Include(p => p.Itens)
                    .Where(p => p.HoraPedido >= inicioUtc && p.HoraPedido < fimUtc);

                if (status != null)
                {
                    consulta = consulta.Where(p => p.Status == status);
                }

                // CalcularTotal() roda em memoria (soma uma lista privada); por isso materializa antes de projetar
                List<Pedido> pedidos = await consulta
                    .OrderByDescending(p => p.HoraPedido)
                    .ToListAsync();

                List<PedidoResumoResponse> resumos = pedidos.Select(p => PedidoResumoResponse.De(p)).ToList();

                return Results.Ok(resumos);
            })
                .Produces<List<PedidoResumoResponse>>(StatusCodes.Status200OK);

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
                Pedido pedido = new Pedido(
                    request.NomeCliente,
                    request.Telefone,
                    request.FormaDePagamento,
                    request.TrocoPara,
                    request.TipoDeEntrega,
                    request.Endereco,
                    request.Referencia,
                    request.Observacao,
                    restaurante,
                    cardapio);

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

                pedido.Finalizar(); // confere o que depende do total (troco) depois de todos os itens

                contexto.Add(pedido);
                await contexto.SaveChangesAsync(); // o banco gera o Numero pela sequence e o EF le de volta

                // a resposta traz a mensagem pronta e o link wa.me: o app do cliente so precisa abrir o link
                return Results.Created($"/pedidos/{pedido.Id}", PedidoResponse.De(pedido, restaurante.WhatsApp));
            })
                .Produces<PedidoResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            app.MapGet("/pedidos/{id:int}", async (int id, KFContext contexto) =>
            {
                Pedido? pedido = await contexto.Pedidos
                    .Include(p => p.Itens)
                    .Include(p => p.CardapioDoDia)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return Results.Problem(detail: $"O pedido com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                // usa o numero atual da loja: permite reenviar o pedido se o cliente fechou o WhatsApp sem mandar
                string? whatsAppDaLoja = await contexto.Restaurantes.OrderBy(r => r.Id).Select(r => r.WhatsApp).FirstOrDefaultAsync();

                return Results.Ok(PedidoResponse.De(pedido, whatsAppDaLoja));
            })
                .Produces<PedidoResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            // chamado pelo front quando o cliente toca em "Enviar no WhatsApp"
            app.MapPost("/pedidos/{id:int}/enviado", async (int id, KFContext contexto) =>
            {
                Pedido? pedido = await contexto.Pedidos.FindAsync(id);

                if (pedido == null)
                {
                    return Results.Problem(detail: $"O pedido com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                pedido.MarcarComoEnviado();
                await contexto.SaveChangesAsync();

                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
