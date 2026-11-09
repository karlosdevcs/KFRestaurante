# KF Restaurante

API de pedidos para um restaurante de marmitas. O dono cadastra as opções, monta e publica o cardápio do dia e define como cada tamanho de marmita pode ser montado. O cliente faz o pedido escolhendo itens avulsos (uma Coca, uma porção de feijoada) ou marmitas montadas (tamanho + componentes).

O pedido termina no **WhatsApp da loja**: a API devolve a mensagem pronta e um link `wa.me`. O cliente toca no link e só aperta enviar. A mensagem traz tudo o que a loja precisa para começar a preparar, sem nenhuma pergunta de volta.

Projeto de estudo e portfólio: o foco está menos na quantidade de funcionalidades e mais em **modelar bem o domínio** e tomar decisões conscientes de persistência e de API.

**Stack:** .NET 8 · ASP.NET Core Minimal API · EF Core 8 · PostgreSQL (Npgsql) · xUnit · Swagger

---

## Como rodar

**Pré-requisitos:** .NET 8 SDK, PostgreSQL e a ferramenta do EF Core (`dotnet tool install --global dotnet-ef --version 8.0.11`).

1. **Connection string.** A senha do banco não fica no repositório: o `appsettings.json` tem só a parte pública. Guarde a connection string completa no user-secrets:

   ```bash
   dotnet user-secrets set "ConnectionStrings:KFRestaurante" "Host=localhost;Port=5432;Database=kfrestaurante;Username=postgres;Password=SUA_SENHA"
   ```

2. **Banco.** Cria o banco e aplica todas as migrations:

   ```bash
   dotnet ef database update
   ```

3. **API.** O perfil `KFRestaurante` sobe em ambiente Development, que é onde o user-secrets é carregado:

   ```bash
   dotnet run --launch-profile KFRestaurante
   ```

   Swagger: http://localhost:5000/swagger

   Se o front rodar em outro endereço, inclua-o em `Cors:OrigensPermitidas` no `appsettings.json`. Já vêm liberadas as portas mais comuns de desenvolvimento (5173, 3000 e 5500).

4. **Testes:**

   ```bash
   dotnet test KFRestaurante.sln
   ```

### Fluxo mínimo para fazer um pedido

1. `POST /opcoes` e `POST /tamanhos`: cadastra o que é vendido.
2. `POST /cardapios` com os ids das opções e depois `POST /cardapios/{id}/publicar`.
3. `PUT /restaurante/whatsapp` e `POST /restaurante/abrir`. O restaurante só abre com o WhatsApp cadastrado.
4. `POST /pedidos`: a resposta traz `mensagemWhatsApp` e `linkWhatsApp`.

---

## Endpoints

| Recurso | Rota | O que faz |
|---|---|---|
| **Opções** | `GET /opcoes` · `GET /opcoes/{id}` | Lista / busca |
| | `POST /opcoes` | Cadastra (nome, categoria, preço opcional) |
| | `PUT /opcoes/{id}` | Edita nome, categoria e preço |
| | `PATCH /opcoes/{id}/esgotado` | Marca ou desmarca como esgotada |
| **Tamanhos** | `GET /tamanhos` · `GET /tamanhos/{id}` | Lista / busca, com os limites por categoria |
| | `POST /tamanhos` · `PUT /tamanhos/{id}` | Cadastra / edita nome, preço e limite total de componentes |
| | `PUT /tamanhos/{id}/limites` | Define os limites por categoria (ex.: até 2 carnes) |
| **Cardápio do dia** | `GET /cardapios/hoje` | **Ponto de entrada do cliente:** o cardápio de hoje, se já publicado |
| | `POST /cardapios` | Cria o cardápio de hoje a partir de ids de opções |
| | `GET /cardapios/{id}` | Busca |
| | `POST /cardapios/{id}/publicar` · `POST /cardapios/{id}/fechar` | Abre / encerra as vendas (publicar de novo reabre) |
| **Restaurante** | `GET /restaurante` · `POST /restaurante/abrir` · `POST /restaurante/fechar` | Estado global: fechado bloqueia pedidos |
| | `PUT /restaurante/whatsapp` | Número da loja, para onde o pedido é enviado |
| **Pedidos** | `POST /pedidos` · `GET /pedidos/{id}` | Cria / busca, com a mensagem e o link do WhatsApp |
| | `GET /pedidos?data=&status=` | Painel da loja: pedidos de um dia (hoje por padrão), do mais recente ao mais antigo, com filtro opcional por status |
| | `POST /pedidos/{id}/enviado` | O front chama quando o cliente toca em "Enviar no WhatsApp" |

Exemplo de pedido com um item avulso e uma marmita:

```json
POST /pedidos
{
  "nomeCliente": "Maria",
  "cardapioId": 3,
  "formaDePagamento": "Dinheiro",
  "trocoPara": 100,
  "tipoDeEntrega": "Entrega",
  "endereco": "Rua A, 10",
  "referencia": "portão azul, ao lado da padaria",
  "observacao": "sem cebola na feijoada",
  "itens": [
    { "opcaoId": 7, "quantidade": 1 },
    { "tamanhoId": 2, "componentesIds": [1, 1, 11], "quantidade": 2 }
  ]
}
```

A mensagem que chega à loja (campo `mensagemWhatsApp` da resposta):

```
*Pedido nº 14*
08/11/2026 às 20:00
Cliente: Maria

1x Feijoada — R$ 25,00
2x Marmita M — R$ 40,00
   Arroz, Arroz, Feijão Tropeiro

*Total: R$ 65,00*

Pagamento: Dinheiro – troco para R$ 100,00
Entrega: Rua A, 10
Referência: portão azul, ao lado da padaria

Obs.: sem cebola na feijoada
```

Todo erro volta no formato padrão **ProblemDetails** (RFC 9110), com a mensagem dizendo o que estava errado e com qual valor:

```json
{ "status": 400, "title": "Regra de negócio violada.", "detail": "A Marmita M aceita no máximo 1 de Carne. Porção dupla conta como 2." }
```

---

## Decisões de projeto

### O pedido chega completo no WhatsApp, sem perguntas

O critério para cada campo do pedido foi: *"se faltar, a loja vai precisar perguntar?"*. Por isso o pedido tem nome do cliente, troco (e "sem troco" escrito por extenso quando não precisa), referência do endereço e observação livre.

O envio usa o link **click-to-chat (`wa.me`)**, e não a API oficial da Meta. A API monta o texto, e o WhatsApp do próprio cliente abre com a mensagem pronta. Não tem custo nem aprovação, e a loja recebe do número do cliente, podendo responder direto. Bibliotecas não oficiais foram descartadas porque violam os termos do WhatsApp e podem levar ao banimento do número da loja. A contrapartida é que o sistema não confirma o envio: o relatório conta pedidos **gerados**.

A montagem da mensagem fica fora do domínio (`Servicos/MensagemWhatsApp`): é formato de saída, não regra de negócio. Ela usa valores em formato brasileiro e horário de Brasília em qualquer servidor, e o restaurante só abre com o número da loja cadastrado.

### O pedido guarda uma foto, não uma referência

O `ItemDoPedido` copia **nome, preço unitário e componentes** no momento da compra, em vez de apontar para a opção.

**Por quê:** o dono muda preços e cardápio o tempo todo. Se o item apontasse para a opção, aumentar o preço da Coca amanhã mudaria o total dos pedidos de ontem, e o relatório mentiria sobre o passado. A foto inclui os componentes da marmita (ex.: `"Arroz, Arroz, Feijoada"`), porque é isso que a cozinha precisa saber, e a marmita em si não é persistida: ela existe só enquanto o pedido é montado.

### Pedido é a raiz do agregado

Só o `Pedido` cria itens (`AdicionarItem`), e é ele quem aplica as regras: restaurante aberto, cardápio publicado, opção no cardápio, opção não esgotada, marmita não vazia, endereço na entrega. No banco, o item **não existe sem o pedido** (chave estrangeira obrigatória, apagado em cascata junto com ele).

### Regras no domínio, não no endpoint

As entidades não têm `set` público: mudar estado só por método (`Publicar()`, `Fechar()`, `AlterarEsgotado()`, `DefinirLimites()`). Assim não existe caminho para, por exemplo, publicar um cardápio vazio escrevendo `cardapio.Situacao = Publicado`.

Os endpoints fazem só três coisas, nesta ordem: validam o formato do request, buscam no banco o que já existe (nunca criam `Opcao` ou `TamanhoMarmita` a partir do JSON) e entregam ao domínio. A persistência é configurada por Fluent API no `KFContext`, sem atributos de banco no domínio.

### Erros centralizados com ProblemDetails

Violação de regra lança `DominioException`. Um `IExceptionHandler` global traduz:

| Exceção | Status |
|---|---|
| `DominioException` | 400, com a mensagem da regra |
| JSON malformado ou com tipo errado | 400 |
| Violação de índice único no Postgres (`23505`) | 409 |
| Qualquer outra | 500, sem stack trace no corpo (o detalhe vai só para o log) |

Os endpoints não têm `try/catch`.

### Unicidade nas duas pontas

Nomes de opção e de tamanho e a data do cardápio são únicos **no banco** (índice único) **e no endpoint** (`AnyAsync` antes de gravar). O `Any` dá uma mensagem clara no caso comum. O índice garante a regra quando dois requests chegam ao mesmo tempo: num teste com 10 requests simultâneos, 4 foram barrados pelo `Any` e 5 pelo índice, sem nenhum duplicado e sem nenhum 500.

### Número do pedido gerado pelo banco

O número sai de uma *sequence* do PostgreSQL, não de "maior número + 1" no código, que daria o mesmo número a dois pedidos simultâneos.

### O dono configura a marmita: limites são dados, não código

Cada tamanho tem um limite total de componentes e, opcionalmente, limites por categoria (*até 2 carnes, até 1 salada*), editáveis pela API. Categoria sem limite fica livre dentro do total. **Porção dupla conta como 2** no limite da categoria. A mudança vale dali em diante, e os pedidos antigos não mudam, por causa da foto.

### Preço opcional: quem vende avulso e quem é só componente

`Preco` nulo significa que a opção só entra em marmita (o arroz). Com preço, ela pode ser vendida sozinha e também entrar em marmita, onde vale o preço do tamanho. Bebida e sobremesa não entram em marmita e, por isso, exigem preço.

### Apagar não pode destruir histórico

As chaves estrangeiras usam `Restrict` onde apagar levaria junto dados que importam: apagar um cardápio não apaga os pedidos do dia, e apagar um tamanho não apaga marmitas. O banco recusa a exclusão.

### Detalhes que evitam bugs silenciosos

- **"Hoje" é o hoje de Brasília:** o servidor pode estar em qualquer fuso (em nuvem, quase sempre UTC). Com `DateTime.Now`, depois das 21h o servidor já estaria no dia seguinte, e o cliente não acharia o cardápio. Toda data de negócio passa por `HorarioBrasilia`.
- **Hora em UTC no banco:** o pedido grava `DateTime.UtcNow`. O PostgreSQL (`timestamptz`) não aceita hora local, e a conversão para Brasília fica para quem exibe.
- **Enums com número fixo:** as categorias têm valores explícitos (`Carne = 0`, `Acompanhamento = 1`...). É o número que vai para o banco, e renumerar mudaria o significado dos dados já gravados.
- **Idempotência:** publicar um cardápio já publicado, fechar o que já está fechado ou abrir o restaurante já aberto não dá erro. Se a rede falhar e o app reenviar, o dono não vê "falhou" sobre algo que funcionou.
- **Migrations revisadas à mão:** cada migration foi lida antes de aplicar. Numa delas, o `Down` gerado pelo EF apagava a sequence antes do default que dependia dela, e o rollback falharia. Em outras, foram acrescentados passos de dados (por exemplo, converter preço `0` em "sem preço" e voltar no rollback).

### DTOs sem AutoMapper

Requests e responses são `record` posicionais em `DTOs/`. A conversão é explícita, por um método estático `De` no próprio response (`OpcaoResponse.De(opcao)`). É fácil de ler, sem mágica e sem expor a entidade na API.

---

## Limitações conhecidas

- **A área da loja não tem autenticação.** Os endpoints do dono (cadastrar opções, publicar cardápio, abrir o restaurante, alterar o WhatsApp) estão abertos, como os do cliente. É uma escolha consciente para o escopo de portfólio. Antes de uso real, esses endpoints precisam de login, e o primeiro passo seria separá-los num grupo de rotas protegido.
- **O sistema não confirma o envio pelo WhatsApp.** O link abre a conversa com a mensagem pronta, mas quem aperta "enviar" é o cliente. Por isso o relatório conta pedidos *gerados*.
- **Não há endpoints de exclusão.** Opções, tamanhos e cardápios só são criados e editados. As chaves estrangeiras já estão preparadas (`Restrict`) para quando a exclusão existir.

## Estrutura

```
Dominio/            entidades e regras de negócio (sem dependência de EF ou HTTP)
DTOs/               requests e responses da API
Endpoints/          rotas agrupadas por recurso (MapOpcoes, MapPedidos...)
Infraestrutura/     KFContext (EF Core, Fluent API) e o tratamento global de erros
Servicos/           montagem da mensagem e do link do WhatsApp
Migrations/         histórico do esquema do banco
KFRestaurante.Tests/  testes de unidade do domínio (xUnit)
Program.cs          só configuração e registro das rotas
```

## Testes

104 testes de unidade cobrem o domínio e a mensagem do WhatsApp: o caminho feliz e cada regra que recusa (restaurante fechado, cardápio não publicado, opção esgotada, limite de componentes, porção dupla, preço congelado, troco menor que o total, entre outras), além do conteúdo exato da mensagem que a loja recebe. Eles não usam banco e rodam em menos de um segundo.
