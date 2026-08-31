# Revisão de Conceitos .NET / C# — Projeto KF Restaurante

Revisão consolidada de tudo que foi construído até aqui. Agrupado por tema, com o essencial de cada conceito e exemplos do próprio projeto. Feito pra reler antes de retomar.

---

## 1. Fundamentos de classe

**Classe × Objeto**
A classe é o **molde** (o desenho). O objeto é a **coisa real**, criada com `new`.
Você chama métodos e lê propriedades sempre no **objeto** (a variável, minúscula: `marmita1`), **nunca** na classe (o molde: `Marmita`).
> Erro mais comum: escrever `Marmita.AdicionarOpcao(...)` (classe) em vez de `marmita1.AdicionarOpcao(...)` (objeto).

**Campo × Propriedade**
Os dois guardam dados, mas a **propriedade** tem "porteiro" (`get`/`set`); o **campo** é gaveta crua.
Prefira propriedade — ela reserva o direito de pôr regra na entrada depois.
- `{ get; set; }` → lê e escreve de qualquer lugar
- `{ get; private set; }` → lê de fora, só a classe escreve
- `{ get; }` → só leitura

**`this`**
É o "eu" do objeto — o próprio objeto, disponível em **qualquer** método da classe (não só no construtor).
Com ele você lê/muda propriedades (`this.Situacao = ...`) e chama outros métodos.
De **dentro** da classe: `this.Situacao`. De **fora**: `marmita1.Situacao`. É a mesma propriedade, vista de dentro e de fora.

---

## 2. Construindo objetos (o raciocínio do construtor)

Pra cada dado, pergunte: **"de onde esse valor vem?"**

| Origem | Vira parâmetro? | O construtor faz o quê? | Exemplo |
|---|---|---|---|
| **Vem de fora e varia** | sim | guarda: `this.X = x` | `this.Opcao = opcao` |
| **É fixo** (igual pra todos) | não | crava na mão | `this.Situacao = SituacaoCardapio.Rascunho` |
| **É derivado** (calculado de outro) | não | vira propriedade calculada | `Subtotal => Quantidade * PrecoUnitario` |

**A pegadinha:** FIXO e DERIVADO os dois ficam fora do parêntese — mas por motivos opostos. Fixo fica fora porque **não varia**; derivado fica fora porque **é calculado** (varia, mas ninguém digita). O que têm em comum: ninguém os passa de fora.

**Invariante** — o objeto não nasce inválido. O construtor garante isso (o cardápio nasce sempre Rascunho; o item nasce com preço já congelado).

---

## 3. Tipos e coleções

**Enum** — lista fixa de valores nomeados (`Categoria`, `SituacaoCardapio`). O compilador vira guardião: só aceita valores da lista.
> Cuidado: o valor padrão de um enum é o **primeiro membro**. Se não definir, assume esse (bug silencioso).

**`decimal`** — sempre pra dinheiro. É preciso em base 10; `double`/`float` arredondam e vazam centavo.

**`DateOnly`** — data sem hora. Escolhido pelo conceito: um cardápio "do dia" não precisa de hora.

**Parâmetro opcional** — valor padrão com `=` na assinatura: `decimal preco = 0`. Quem chama pode passar ou omitir. Se omitir, assume o padrão. (Usado na `Opcao`: bebida passa preço, componente omite.) Parâmetros opcionais vêm sempre **depois** dos obrigatórios.

**Vitrine + estoque** (só pra coleções que a classe protege):
- **estoque**: `private List<T> _x = new List<T>()` — a lista real, rica (tem `.Add`/`.Remove`), escondida
- **vitrine**: `public IEnumerable<T> X => _x` — só deixa percorrer (foreach), **sem** `.Add`
- **método** (`AdicionarX`): a única porta pra modificar o estoque — onde ficam as regras

`List` = ver + contar + modificar. `IEnumerable` = só percorrer. Você usa a **poderosa (List) escondida** por dentro e a **fraca (IEnumerable) exposta** por fora.
> **Quando usar:** coleção que a classe modifica por dentro mas quer proteger por fora (Catalogo, CardapioDoDia, Marmita).
> **Regra:** não se pode dar `new` numa interface (`IEnumerable`); o objeto real é sempre a `List`, a interface é só o rótulo por cima.

---

## 4. Comportamento e regras

**Método** — o que a classe *sabe fazer*.
- Parêntese **vazio** `()` → a ação não precisa de nada de fora (`Publicar()`)
- Parêntese **com peça** `(coisa)` → precisa de algo de fora (`AdicionarOpcao(opcao)`)
> Teste: "o objeto consegue fazer isso só com o que já tem?" Sim → `()`. Falta peça de fora → `(peça)`. A peça **nunca** é o próprio objeto.

**Método com regra de negócio** — o método protege uma regra que ninguém consegue furar:
- `Publicar()` → só publica se o cardápio não estiver vazio
- `AdicionarOpcao()` → recusa opção duplicada

**Propriedade calculada** (`=>`) — deriva um valor na hora da leitura, **não guarda**:
- `Subtotal => Quantidade * PrecoUnitario`
- `PrecoMarmita => Tamanho.Preco`
Nunca desatualiza (se a quantidade mudar, o próximo cálculo já reflete). **Não entra no construtor.**

**Congelamento de preço** — o item **copia** o preço no momento de nascer:
`this.PrecoUnitario = opcao.Preco;` (com `private set`, pra ninguém alterar a foto depois).
Se a opção mudar de preço amanhã, o item de ontem **não muda** — guardou a foto do momento.
> Contraste importante: o **PrecoUnitario** é guardado (é uma foto, tem que ficar fixo). O **Subtotal** é calculado (deriva de outros). Cada um pelo motivo certo.

---

## 5. Exceções

**`throw`** — *lança* o erro (grita "deu errado" e para o método). Vai **onde a regra é violada**, dentro do domínio:
`throw new InvalidOperationException("mensagem clara");`

**`try/catch`** — *pega* o erro (a rede de segurança). Vai em **quem chama** o método (o Main), não no domínio:
`try { ... } catch (Exception e) { Console.WriteLine(e.Message); }`

**A dupla, lados opostos:** o domínio **lança** (`throw`), quem usa **pega** (`try/catch`). Um joga a bola, o outro pega. Nunca ponha `try/catch` dentro do próprio método que lança — senão ninguém fica sabendo do erro.

**`throw` × `return`:** `return` volta em silêncio (o método termina como se estivesse tudo bem). `throw` interrompe gritando. Pra regra que não pode ser violada, é `throw`.

---

## 6. Busca na mão (antes do LINQ)

Pra procurar um item numa coleção com `foreach` + `if`:

**Bandeira** (`bool jaExiste`) — variável que "lembra" se o loop achou algo. Começa `false`, vira `true` no achado.

**Regra de ouro:** o loop **procura**; a decisão (throw / add) vem **depois** do loop, **fora** dele.
> Se você decide dentro do loop, decide com informação incompleta (só viu parte da lista). Ex.: adicionar dentro do loop faz o item entrar uma vez por elemento existente.

---

## 7. Arquitetura (a cebola)

```
Site        →  o rosto (o cliente vê)
API / CRUD  →  a porta HTTP (o mundo externo entra)
Domínio     →  as regras (o coração)   ← onde você está
Banco       →  a memória (persiste os dados)
```

**Direção da dependência:** tudo aponta **pra dentro**, pro domínio. O domínio **não depende de ninguém** — não sabe que existe site, API ou banco.
> Por isso o domínio **não fala com a tela**: nada de `Console.WriteLine` numa regra de negócio. Ele lança exceção; quem usa é que decide como mostrar.
> Princípio: *estrutura se conquista, não se instala.* Cada camada nasce quando uma necessidade real a exige, não por calendário.

---

## Bandeiras de refatoração (anotadas, não esquecidas)

1. O `foreach` de busca está **duplicado** no `Catalogo` e no `CardapioDoDia`. Resolve com **composição** um dia (não com herança — herança é pra "é um", não pra "compartilhar código").
2. A distinção "bebida tem preço / componente não tem" foi resolvida meio na força (o `preco = 0` opcional na `Opcao`). Revisitar quando fizer sentido.

---

## Onde o projeto está

**Mundo do dono — completo ✅**
- `Opcao` (com `Preco` opcional), `Categoria` (enum), `TamanhoMarmita`
- `Catalogo` (vitrine+estoque + recusa duplicata)
- `CardapioDoDia` (data, situação, `Publicar()` com regra, recusa duplicata)

**Mundo do cliente — em construção 🔨**
- ✅ `ItemDoPedido` (opção avulsa, preço congelado, subtotal calculado)
- ✅ `Marmita` (tamanho + componentes, preço derivado do tamanho)
- ⬜ **Próximo: `Pedido`** — junta os itens, calcula o total, guarda forma de pagamento e tipo de recebimento

**Ainda adiante:** LINQ (provavelmente dentro do Pedido), banco (EF Core), API (ASP.NET Core), frontend.

---

## Ritual de 10 segundos antes de escrever (pra cortar os erros bobos)

1. **Objeto ou classe?** Estou usando a variável (`marmita1`) ou o molde (`Marmita`)?
2. **De onde vem o dado?** Vem de fora / é fixo / é derivado? (o quadro CONSTRUINDO OBJETOS)
3. **Já fiz algo parecido?** Qual padrão conhecido encaixa aqui?

---

*Consolidado até a classe `Marmita`. Amanhã: o `Pedido`.*
