using KFRestaurante.Dominio;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace KFRestaurante.Infraestrutura
{
    // pega qualquer excecao que escapar de um endpoint e devolve ProblemDetails
    public class ManipuladorDeExcecoes : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ManipuladorDeExcecoes> _logger;

        public ManipuladorDeExcecoes(IProblemDetailsService problemDetailsService, ILogger<ManipuladorDeExcecoes> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails;

            if (exception is DominioException)
            {
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Regra de negócio violada.",
                    Detail = exception.Message
                };
            }
            // JSON malformado ou com tipo errado (ex.: "quantidade": "dois"). Em Development o minimal API lanca em vez de responder 400 sozinho
            else if (exception is BadHttpRequestException requisicaoInvalida)
            {
                problemDetails = new ProblemDetails
                {
                    Status = requisicaoInvalida.StatusCode,
                    Title = "Requisição inválida.",
                    Detail = "O corpo da requisição não está no formato esperado."
                };
            }
            // 23505 = unique_violation: dois requests passaram pelo Any ao mesmo tempo e o indice unico barrou o segundo
            else if (exception is DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres })
            {
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Registro duplicado.",
                    Detail = $"Já existe um registro com esse valor ({postgres.ConstraintName})."
                };
            }
            else
            {
                // o detalhe vai so pro log; o cliente nunca ve stack trace nem mensagem interna
                _logger.LogError(exception, "Erro inesperado em {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);

                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erro inesperado.",
                    Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde."
                };
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
        }
    }
}
