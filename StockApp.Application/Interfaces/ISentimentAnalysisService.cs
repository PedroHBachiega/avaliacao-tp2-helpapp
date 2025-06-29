using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de análise de sentimento
    /// </summary>
    public interface ISentimentAnalysisService
    {
        /// <summary>
        /// Analisa o sentimento de um texto
        /// </summary>
        /// <param name="text">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento (Positivo, Negativo, Neutro)</returns>
        string AnalyzeSentiment(string text);

        /// <summary>
        /// Analisa o sentimento de um texto de forma assíncrona
        /// </summary>
        /// <param name="text">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento (Positivo, Negativo, Neutro)</returns>
        Task<string> AnalyzeSentimentAsync(string text);
    }
}