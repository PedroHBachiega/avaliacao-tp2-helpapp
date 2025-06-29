using System.ComponentModel.DataAnnotations;

namespace StockApp.API.Models
{
    public class SentimentAnalysisRequest
    {
        [Required(ErrorMessage = "O texto para análise é obrigatório")]
        public string Text { get; set; }
    }
}