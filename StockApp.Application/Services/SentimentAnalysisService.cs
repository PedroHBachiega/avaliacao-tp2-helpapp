using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StockApp.Application.Interfaces;

namespace StockApp.Application.Services
{
    /// <summary>
    /// Implementação do serviço de análise de sentimento
    /// </summary>
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        // Listas de palavras positivas e negativas para análise de sentimento
        private readonly HashSet<string> _positiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "bom", "ótimo", "excelente", "incrível", "maravilhoso", "fantástico", "perfeito", "adorei", "gostei",
            "recomendo", "satisfeito", "feliz", "contente", "agradável", "útil", "eficiente", "rápido", "qualidade",
            "durável", "confiável", "brilhante", "impressionante", "surpreendente", "satisfatório", "positivo",
            "good", "great", "excellent", "amazing", "wonderful", "fantastic", "perfect", "loved", "liked",
            "recommend", "satisfied", "happy", "pleased", "pleasant", "useful", "efficient", "fast", "quality",
            "durable", "reliable", "brilliant", "impressive", "surprising", "satisfactory", "positive"
        };

        private readonly HashSet<string> _negativeWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ruim", "péssimo", "terrível", "horrível", "decepcionante", "insatisfeito", "problema", "defeito",
            "quebrado", "lento", "caro", "difícil", "complicado", "frágil", "fraco", "ineficiente", "inútil",
            "desapontado", "frustrado", "irritado", "negativo", "falha", "erro", "pobre", "baixa qualidade",
            "bad", "terrible", "horrible", "disappointing", "unsatisfied", "problem", "defect",
            "broken", "slow", "expensive", "difficult", "complicated", "fragile", "weak", "inefficient", "useless",
            "disappointed", "frustrated", "annoyed", "negative", "failure", "error", "poor", "low quality"
        };

        /// <summary>
        /// Analisa o sentimento de um texto
        /// </summary>
        /// <param name="text">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento (Positivo, Negativo, Neutro)</returns>
        public string AnalyzeSentiment(string text)
        {
            // Se o texto for nulo ou vazio, retorna Neutro
            if (string.IsNullOrEmpty(text))
                return "Neutro";

            // Normaliza o texto (remove acentos, converte para minúsculas)
            string normalizedText = NormalizeText(text);

            // Divide o texto em palavras
            var words = normalizedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Conta palavras positivas e negativas
            int positiveCount = words.Count(w => _positiveWords.Contains(w));
            int negativeCount = words.Count(w => _negativeWords.Contains(w));

            // Se não encontrou nenhuma palavra de sentimento, retorna Neutro
            if (positiveCount == 0 && negativeCount == 0)
                return "Neutro";

            // Calcula o score de sentimento
            double sentimentScore = CalculateSentimentScore(positiveCount, negativeCount, words.Count);

            // Determina o sentimento com base no score
            return DetermineSentiment(sentimentScore);
        }

        /// <summary>
        /// Analisa o sentimento de um texto de forma assíncrona
        /// </summary>
        /// <param name="text">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento (Positivo, Negativo, Neutro)</returns>
        public async Task<string> AnalyzeSentimentAsync(string text)
        {
            return await Task.Run(() => AnalyzeSentiment(text));
        }

        /// <summary>
        /// Calcula o score de sentimento com base na contagem de palavras positivas e negativas
        /// </summary>
        private double CalculateSentimentScore(int positiveCount, int negativeCount, int totalWords)
        {
            if (totalWords == 0)
                return 0;

            // Calcula o score normalizado entre -1 e 1
            double positiveRatio = (double)positiveCount / totalWords;
            double negativeRatio = (double)negativeCount / totalWords;

            // Se temos tanto palavras positivas quanto negativas, é mais provável que seja neutro
            if (positiveCount > 0 && negativeCount > 0)
            {
                // Reduzir o score para casos mistos
                return (positiveRatio - (negativeRatio * 1.2)) * 0.8;
            }
            
            // Dando um peso mais equilibrado para palavras negativas (1.2x)
            return positiveRatio - (negativeRatio * 1.2);
        }

        /// <summary>
        /// Determina o sentimento com base no score
        /// </summary>
        private string DetermineSentiment(double score)
        {
            // Ajustando os limiares para melhor classificação
            if (score > 0.25)  // Aumentando o limiar para positivo para evitar falsos positivos
                return "Positivo";
            else if (score < -0.25)  // Aumentando o limiar para negativo para evitar falsos negativos
                return "Negativo";
            else
                return "Neutro";
        }

        /// <summary>
        /// Normaliza o texto removendo acentos e convertendo para minúsculas
        /// </summary>
        /// <param name="text">Texto a ser normalizado</param>
        /// <returns>Texto normalizado</returns>
        private string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Converte para minúsculas
            string normalizedText = text.ToLower();

            // Remove acentos
            normalizedText = Regex.Replace(normalizedText, @"\p{M}", "");

            // Remove caracteres especiais e substitui por espaço
            normalizedText = Regex.Replace(normalizedText, @"[^a-z0-9\s]", " ");

            // Remove espaços extras
            normalizedText = Regex.Replace(normalizedText, @"\s+", " ").Trim();

            return normalizedText;
        }
    }
}