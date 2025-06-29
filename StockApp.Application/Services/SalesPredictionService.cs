using Microsoft.ML;
using Microsoft.ML.Data;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Application.Services
{
    public class SalesPredictionService : ISalesPredictionService
    {
        private readonly IProductRepository _productRepository;
        private readonly MLContext _mlContext;
        
        public SalesPredictionService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _mlContext = new MLContext(seed: 0);
        }

        public double PredictSales(int productId, int month, int year)
        {
            // Em um cenário real, você carregaria dados históricos de vendas
            // e usaria um modelo treinado para fazer previsões
            
            // Este é um exemplo simplificado que simula uma previsão
            // baseada em um modelo de regressão linear simples
            
            // 1. Criar dados de treinamento simulados
            var trainingData = GenerateTrainingData(productId);
            
            // 2. Carregar dados no IDataView
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            
            // 3. Definir pipeline de treinamento
            var pipeline = _mlContext.Transforms.Concatenate("Features", "Month", "Year")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Sales", maximumNumberOfIterations: 100));
            
            // 4. Treinar o modelo
            var model = pipeline.Fit(dataView);
            
            // 5. Usar o modelo para fazer previsões
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SalesData, SalesPrediction>(model);
            
            var prediction = predictionEngine.Predict(new SalesData
            {
                ProductId = productId,
                Month = (float)month,
                Year = (float)year
            });
            
            // Arredondar para o número inteiro mais próximo e garantir que seja positivo
            return Math.Max(0, Math.Round((double)prediction.PredictedSales, 0));
        }
        
        private IEnumerable<SalesData> GenerateTrainingData(int productId)
        {
            // Em um cenário real, você buscaria dados históricos de vendas do banco de dados
            // Este é apenas um exemplo que gera dados simulados para demonstração
            
            var random = new Random(productId); // Usar productId como seed para consistência
            var baseValue = random.Next(500, 2000); // Valor base de vendas para este produto
            
            var data = new List<SalesData>();
            
            // Gerar dados para os últimos 3 anos
            for (int year = DateTime.Now.Year - 3; year < DateTime.Now.Year; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    // Simular sazonalidade e tendência
                    double seasonality = 1.0 + 0.3 * Math.Sin((month - 1) * Math.PI / 6); // Fator sazonal
                    double trend = 1.0 + 0.1 * (year - (DateTime.Now.Year - 3)); // Tendência de crescimento
                    
                    // Adicionar alguma aleatoriedade
                    double noise = 0.9 + 0.2 * random.NextDouble();
                    
                    double sales = baseValue * seasonality * trend * noise;
                    
                    data.Add(new SalesData
                    {
                        ProductId = productId,
                        Month = (float)month,
                        Year = (float)year,
                        Sales = (float)sales
                    });
                }
            }
            
            return data;
        }
    }
    
    // Classes para ML.NET
    public class SalesData
    {
        public int ProductId { get; set; }
        
        [LoadColumn(0)]
        public float Month { get; set; }
        
        [LoadColumn(1)]
        public float Year { get; set; }
        
        [LoadColumn(2), ColumnName("Label")]
        public float Sales { get; set; }
    }
    
    public class SalesPrediction
    {
        [ColumnName("Score")]
        public float PredictedSales { get; set; }
    }
}