# KF Restaurante

API de pedidos para um restaurante de marmitas. O dono cadastra as opções, monta e publica o cardápio do dia e define como cada tamanho de marmita pode ser montado. O cliente faz o pedido escolhendo itens avulsos (uma Coca, uma porção de feijoada) ou marmitas montadas (tamanho + componentes).

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

4. **Testes:**

   ```bash
   dotnet test KFRestaurante.sln
   ```

### Fluxo mínimo para fazer um pedido

1. `POST /opcoes` e `POST /tamanhos`: cadastra o que é vendido.
2. `POST /cardapios` com os ids das opções e depois `POST /cardapios/{id}/publicar`.
3. `POST /restaurante/abrir`.
4. `POST /pedidos`.

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
| **Cardápio do dia** | `POST /cardapios` | Cria o cardápio de hoje a partir de ids de opções |
| | `GET /cardapios/{id}` | Busca |
| | `POST /cardapios/{id}/publicar` · `POST /cardapios/{id}/fechar` | Abre / encerra as vendas (publicar de novo reabre) |
| **Restaurante** | `GET /restaurante` · `POST /restaurante/abrir` · `POST /restaurante/fechar` | Estado global: fechado bloqueia pedidos |
| **Pedidos** | `POST /pedidos` · `GET /pedidos/{id}` | Cria / busca |

Exemplo de pedido com um item avulso e uma marmita:

```json
POST /pedidos
{
  "cardapioId": 3,
  "formaDePagamento": "Pix",
  "tipoDeEntrega": "Entrega",
  "endereco": "Rua A, 10",
  "itens": [
    { "opcaoId": 7, "quantidade": 1 },
    { "tamanhoId": 2, "componentesIds": [1, 1, 11], "quantidade": 2 }
  ]
}
```

Todo erro volta no formato padrão **ProblemDetails** (RFC 9110), com a mensagem dizendo o que estava errado e com qual valor:

```json
{ "status": 400, "title": "Regra de negócio violada.", "detail": "A Marmita M aceita no máximo 1 de Carne. Porção dupla conta como 2." }
```

---

## Decisões de projeto

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

- **Hora em UTC:** o pedido grava `DateTime.UtcNow`. O PostgreSQL (`timestamptz`) não aceita hora local, e a conversão para o fuso do Brasil fica para quem exibe.
- **Enums com número fixo:** as categorias têm valores explícitos (`Carne = 0`, `Acompanhamento = 1`...). É o número que vai para o banco, e renumerar mudaria o significado dos dados já gravados.
- **Idempotência:** publicar um cardápio já publicado, fechar o que já está fechado ou abrir o restaurante já aberto não dá erro. Se a rede falhar e o app reenviar, o dono não vê "falhou" sobre algo que funcionou.
- **Migrations revisadas à mão:** cada migration foi lida antes de aplicar. Numa delas, o `Down` gerado pelo EF apagava a sequence antes do default que dependia dela, e o rollback falharia. Em outras, foram acrescentados passos de dados (por exemplo, converter preço `0` em "sem preço" e voltar no rollback).

### DTOs sem AutoMapper

Requests e responses são `record` posicionais em `DTOs/`. A conversão é explícita, por um método estático `De` no próprio response (`OpcaoResponse.De(opcao)`). É fácil de ler, sem mágica e sem expor a entidade na API.

---

## Estrutura

```
Dominio/            entidades e regras de negócio (sem dependência de EF ou HTTP)
DTOs/               requests e responses da API
Endpoints/          rotas agrupadas por recurso (MapOpcoes, MapPedidos...)
Infraestrutura/     KFContext (EF Core, Fluent API) e o tratamento global de erros
Migrations/         histórico do esquema do banco
KFRestaurante.Tests/  testes de unidade do domínio (xUnit)
Program.cs          só configuração e registro das rotas
```

## Testes

54 testes de unidade cobrem o domínio: o caminho feliz e cada regra que recusa (restaurante fechado, cardápio não publicado, opção esgotada, limite de componentes, porção dupla, preço congelado, entre outras). Eles não usam banco e rodam em menos de um segundo.
