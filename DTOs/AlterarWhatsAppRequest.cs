namespace KFRestaurante.DTOs
{
    // aceita com ou sem formatacao: "5531999999999" ou "+55 (31) 99999-9999"
    public record AlterarWhatsAppRequest(string WhatsApp);
}
