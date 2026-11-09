using KFRestaurante.Infraestrutura.Autenticacao;
using KFRestaurante.Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace KFRestaurante.Tests.Seguranca
{
    // Sobe a API de verdade (sem banco: nada aqui faz consulta) so para ler a tabela de rotas e conferir
    // quais exigem login e qual permissao cada uma pede. Se alguem criar um endpoint e esquecer de protege-lo,
    // ou trocar a permissao de um existente, este teste falha.
    public class FabricaDaApi : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string?> _anteriores = new();

        public FabricaDaApi()
        {
            // a configuracao e lida no inicio do Program: variaveis de ambiente sao vistas por ele
            Definir("Jwt__Chave", new string('k', 48));
            Definir("ConnectionStrings__KFRestaurante", "Host=localhost;Database=teste_sem_uso;Username=teste");
            Definir("Seed__Superuser__Email", ""); // garante que o seed nao tente abrir conexao
        }

        private void Definir(string nome, string valor)
        {
            _anteriores[nome] = Environment.GetEnvironmentVariable(nome);
            Environment.SetEnvironmentVariable(nome, valor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach ((string nome, string? valor) in _anteriores)
                {
                    Environment.SetEnvironmentVariable(nome, valor);
                }
            }

            base.Dispose(disposing);
        }
    }

    public class EndpointsProtegidosTests : IClassFixture<FabricaDaApi>
    {
        private readonly FabricaDaApi _fabrica;

        public EndpointsProtegidosTests(FabricaDaApi fabrica)
        {
            _fabrica = fabrica;
        }

        // rota -> politica exigida. null = aberta (e o que o CLIENTE usa; nada alem disso pode ser aberto)
        private static string P(Permissao p) => Politicas.De(p);

        private static readonly Dictionary<string, string?> Esperado = new()
        {
            // abertas
            ["POST /auth/login"] = null,
            ["GET /cardapios/hoje"] = null,
            ["GET /restaurante"] = null,
            ["GET /tamanhos"] = null,
            ["GET /tamanhos/{id:int}"] = null,
            ["POST /pedidos"] = null,
            ["POST /pedidos/{id:int}/enviado"] = null,

            // qualquer usuario logado
            ["GET /auth/eu"] = Politicas.Autenticado,
            ["POST /auth/trocar-senha"] = Politicas.Autenticado,
            ["GET /opcoes"] = Politicas.Autenticado,
            ["GET /opcoes/{id:int}"] = Politicas.Autenticado,

            // gerenciar usuarios: dono ou superuser (a hierarquia refina dentro do endpoint)
            ["GET /usuarios"] = Politicas.GerenciarUsuarios,
            ["GET /usuarios/{id:int}"] = Politicas.GerenciarUsuarios,
            ["POST /usuarios"] = Politicas.GerenciarUsuarios,
            ["PUT /usuarios/{id:int}"] = Politicas.GerenciarUsuarios,
            ["POST /usuarios/{id:int}/desativar"] = Politicas.GerenciarUsuarios,
            ["POST /usuarios/{id:int}/ativar"] = Politicas.GerenciarUsuarios,
            ["PUT /usuarios/{id:int}/senha"] = Politicas.GerenciarUsuarios,

            // por permissao
            ["GET /pedidos"] = P(Permissao.VerPedidos),
            ["GET /pedidos/{id:int}"] = P(Permissao.VerPedidos),
            ["PATCH /opcoes/{id:int}/esgotado"] = P(Permissao.EsgotarOpcoes),
            ["POST /restaurante/abrir"] = P(Permissao.AbrirFecharLoja),
            ["POST /restaurante/fechar"] = P(Permissao.AbrirFecharLoja),
            ["POST /cardapios"] = P(Permissao.GerenciarCardapio),
            ["GET /cardapios/{id:int}"] = P(Permissao.GerenciarCardapio),
            ["POST /cardapios/{id:int}/publicar"] = P(Permissao.GerenciarCardapio),
            ["POST /cardapios/{id:int}/fechar"] = P(Permissao.GerenciarCardapio),
            ["POST /opcoes"] = P(Permissao.GerenciarCadastros),
            ["PUT /opcoes/{id:int}"] = P(Permissao.GerenciarCadastros),
            ["POST /tamanhos"] = P(Permissao.GerenciarCadastros),
            ["PUT /tamanhos/{id:int}"] = P(Permissao.GerenciarCadastros),
            ["PUT /tamanhos/{id:int}/limites"] = P(Permissao.GerenciarCadastros),
            ["PUT /restaurante/whatsapp"] = P(Permissao.AlterarWhatsApp),
        };

        // rota real -> politica que ela exige (null = nenhuma)
        private Dictionary<string, string?> RotasDaApi()
        {
            EndpointDataSource fonte = _fabrica.Services.GetRequiredService<EndpointDataSource>();
            Dictionary<string, string?> rotas = new();

            foreach (RouteEndpoint endpoint in fonte.Endpoints.OfType<RouteEndpoint>())
            {
                IReadOnlyList<string> metodos = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? Array.Empty<string>();
                string rota = endpoint.RoutePattern.RawText ?? "?";
                string? politica = endpoint.Metadata.GetMetadata<IAuthorizeData>()?.Policy;
                bool exigeLogin = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Any();

                foreach (string metodo in metodos)
                {
                    rotas[$"{metodo} {rota}"] = exigeLogin ? (politica ?? "(login sem politica)") : null;
                }
            }

            return rotas;
        }

        [Fact]
        public void Todas_as_rotas_da_api_estao_na_tabela_de_seguranca()
        {
            Dictionary<string, string?> reais = RotasDaApi();

            List<string> semDono = reais.Keys.Except(Esperado.Keys).ToList();
            List<string> sumiram = Esperado.Keys.Except(reais.Keys).ToList();

            // rota nova precisa ser classificada aqui: aberta ou protegida, e por qual permissao
            Assert.True(semDono.Count == 0, "Rotas novas sem classificação de segurança: " + string.Join(" | ", semDono));
            Assert.True(sumiram.Count == 0, "Rotas da tabela que não existem mais: " + string.Join(" | ", sumiram));
        }

        [Fact]
        public void Cada_rota_exige_exatamente_a_politica_esperada()
        {
            Dictionary<string, string?> reais = RotasDaApi();

            List<string> divergencias = Esperado
                .Where(par => reais.TryGetValue(par.Key, out string? real) && real != par.Value)
                .Select(par => $"{par.Key}: esperado '{par.Value ?? "aberta"}', real '{reais[par.Key] ?? "aberta"}'")
                .ToList();

            Assert.True(divergencias.Count == 0, string.Join(" | ", divergencias));
        }

        [Fact]
        public void So_as_rotas_do_cliente_e_o_login_ficam_abertas()
        {
            List<string> abertas = RotasDaApi().Where(par => par.Value == null).Select(par => par.Key).OrderBy(r => r).ToList();

            Assert.Equal(
                new[]
                {
                    "GET /cardapios/hoje", "GET /restaurante", "GET /tamanhos", "GET /tamanhos/{id:int}",
                    "POST /auth/login", "POST /pedidos", "POST /pedidos/{id:int}/enviado"
                }.OrderBy(r => r),
                abertas);
        }

        [Fact]
        public void Toda_permissao_tem_uma_politica_registrada()
        {
            Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider provedor =
                _fabrica.Services.GetRequiredService<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider>();

            foreach (Permissao permissao in PermissaoExtensions.Individuais)
            {
                Assert.NotNull(provedor.GetPolicyAsync(Politicas.De(permissao)).GetAwaiter().GetResult());
            }
        }
    }
}
