using System.IdentityModel.Tokens.Jwt;
using KFRestaurante.Dominio;
using KFRestaurante.Endpoints;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Infraestrutura.Autenticacao;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;


namespace KFRestaurante
{
    public class Program
    {
        static async Task Main(string[] args)
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

            builder.Services.AddSwaggerGen(options =>
            {
                // botao "Authorize" no Swagger: cole o token devolvido por POST /auth/login
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Token devolvido por POST /auth/login (sem a palavra 'Bearer')."
                });

                options.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", documento)] = new List<string>()
                });
            });

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ManipuladorDeExcecoes>();

            // ---------- autenticacao e autorizacao (area da loja) ----------
            // a chave do JWT vem do user-secrets; sem ela a API nem sobe (melhor que falhar no primeiro login)
            OpcoesJwt opcoesJwt = builder.Configuration.GetSection(OpcoesJwt.Secao).Get<OpcoesJwt>() ?? new OpcoesJwt();
            opcoesJwt.Validar();

            builder.Services.AddSingleton(opcoesJwt);
            builder.Services.AddSingleton<ServicoDeSenha>();
            builder.Services.AddSingleton<ServicoDeToken>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<UsuarioAtual>();
            builder.Services.AddScoped<IAuthorizationHandler, AutorizacaoHandler>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false; // mantem o claim "sub" com esse nome

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = opcoesJwt.Emissor,
                        ValidateAudience = true,
                        ValidAudience = opcoesJwt.Audiencia,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = opcoesJwt.ChaveDeAssinatura(),
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        // token valido nao basta: o usuario precisa existir e estar ativo AGORA.
                        // Desativar alguem corta o acesso na hora, sem esperar o token vencer.
                        OnTokenValidated = async contexto =>
                        {
                            string? sub = contexto.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                            UsuarioAtual atual = contexto.HttpContext.RequestServices.GetRequiredService<UsuarioAtual>();
                            Usuario? usuario = int.TryParse(sub, out int id) ? await atual.CarregarAsync(id) : null;

                            if (usuario == null || !usuario.Ativo)
                            {
                                contexto.Fail("Usuário inexistente ou inativo.");
                            }
                        }
                    };
                });

            builder.Services.AddAuthorization(Politicas.Registrar);

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
            // 401, 403 e 404 sem corpo tambem saem como ProblemDetails, agora com `detail` em portugues
            // (o padrao do ASP.NET traz so o `title` em ingles) para o front mostrar direto
            app.UseStatusCodePages(async contexto =>
            {
                HttpContext http = contexto.HttpContext;
                int status = http.Response.StatusCode;

                string? detalhe = status switch
                {
                    StatusCodes.Status401Unauthorized => "Você precisa entrar para continuar.",
                    StatusCodes.Status403Forbidden => "Você não tem permissão para fazer isso.",
                    StatusCodes.Status404NotFound => "O endereço pedido não existe.",
                    _ => null
                };

                await http.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = http,
                    ProblemDetails = { Status = status, Detail = detalhe }
                });
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAuth();
            app.MapUsuarios();
            app.MapOpcoes();
            app.MapTamanhos();
            app.MapCardapios();
            app.MapRestaurante();
            app.MapPedidos();

            app.UseSwagger();
            app.UseSwaggerUI();

            // primeiro superuser, a partir da configuracao local. Falha aqui nao derruba a API: so avisa no log.
            try
            {
                await SeedDeUsuarios.GarantirSuperuserAsync(app.Services, app.Configuration, app.Logger);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Não foi possível criar o superuser inicial. A migration de autenticação foi aplicada?");
            }

            app.Run();
        }
    }
}
