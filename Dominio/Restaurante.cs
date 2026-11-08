using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFRestaurante.Dominio
{
    public class Restaurante
    {
        public int Id { get; private set; }
        public bool Aberto { get; private set; }

        // para onde o link wa.me do pedido aponta. So digitos, com pais e DDD: 5531999999999
        public string? WhatsApp { get; private set; }

        public Restaurante()
        {
            this.Aberto = false;
        }

        public void AlterarWhatsApp(string numero)
        {
            // aceita "+55 (31) 99999-9999" e guarda "5531999999999"
            string digitos = new string((numero ?? "").Where(char.IsDigit).ToArray());

            if (digitos.Length < 12 || digitos.Length > 13)
            {
                throw new DominioException($"WhatsApp inválido: '{numero}'. Informe com código do país e DDD, ex.: 5531999999999.");
            }

            this.WhatsApp = digitos;
        }

        public void Abrir()
        {
            // sem numero, o cliente montaria o pedido e nao teria para onde envia-lo
            if (this.WhatsApp == null)
            {
                throw new DominioException("Cadastre o WhatsApp da loja antes de abrir.");
            }

            this.Aberto = true; // ja aberto: continua aberto, sem erro (idempotente)
        }

        public void Fechar()
        {
            this.Aberto = false; // ja fechado: continua fechado, sem erro (idempotente)
        }

    }
}
