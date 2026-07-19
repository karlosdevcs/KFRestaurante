SPEC v1 — KF Restaurante
Catálogo (mundo do dono, existe antes de qualquer cliente):

Opção — item cadastrado: nome, categoria, estado esgotado. Ex: tropeiro, coca-cola. Bebida/sobremesa tem preço próprio; componente de marmita, não.
Tamanho de Marmita — P/M/G, cada um com seu preço. É daqui que vem o preço da marmita.
Categoria — lista fixa: prato, acompanhamento, bebida, sobremesa.
Cardápio do Dia — data, opções disponíveis, estado (rascunho/publicado), registro da última edição.
Restaurante — aberto/fechado (estado global).

Movimento (nasce da ação do cliente):

Pedido — número, data/hora, itens, forma de pagamento, tipo de recebimento (retirada ou entrega + endereço em texto), valor total. Com a sua escolha do relatório, ele nasce pronto e não muda mais — era a surpresa da pergunta 4: seu Pedido ficou simples.
Item do Pedido — quantidade, preço congelado no momento, e o que foi pedido: uma referência direta a uma Opção (a coca) ou uma marmita montada (tamanho + opções escolhidas). Hipótese de trabalho: marmita não existe no catálogo, só montada dentro do pedido. O código confirma ou derruba.

Listas fixas: Forma de Pagamento (Pix, dinheiro, cartão) e Tipo de Recebimento (retirada, entrega) — a categoria que faltava na sua triagem.
Regras:

Restaurante fechado bloqueia a geração de pedido (não só avisa).
Opção esgotada fica indisponível na tela pública.
O relatório do dia conta pedidos gerados — nome honesto, o sistema não enxerga o WhatsApp.
O pedido congela o preço do momento. Se o dono aumentar o preço amanhã, os pedidos de hoje não mudam — era a resposta da pergunta traiçoeira; sem isso, seu relatório mentiria sobre o passado.
Cardápio publicado pode ser editado, guardando a última edição.