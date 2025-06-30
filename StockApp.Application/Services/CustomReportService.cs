using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class CustomReportService : ICustomReportService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CustomReportService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<CustomReportDto> GenerateReportAsync(ReportParametersDto parameters)
        {
            // Implementação da geração de relatórios personalizados baseada nos parâmetros
            var customReport = new CustomReportDto
            {
                Title = "Relatório Personalizado",
                GeneratedAt = DateTime.UtcNow,
                Description = $"Relatório gerado com base nos parâmetros: {parameters.ReportType}"
            };

            switch (parameters.ReportType?.ToLower())
            {
                case "vendas":
                    await GenerateSalesReport(customReport, parameters);
                    break;
                case "estoque":
                    await GenerateStockReport(customReport, parameters);
                    break;
                case "categoria":
                    await GenerateCategoryReport(customReport, parameters);
                    break;
                case "produto":
                    await GenerateProductReport(customReport, parameters);
                    break;
                default:
                    // Relatório padrão com informações básicas
                    customReport.Data.Add(new ReportDataDto { Key = "TotalVendas", Value = "10000" });
                    customReport.Data.Add(new ReportDataDto { Key = "TotalPedidos", Value = "200" });
                    break;
            }

            return customReport;
        }

        private async Task GenerateSalesReport(CustomReportDto report, ReportParametersDto parameters)
        {
            var random = new Random();
            report.Title = "Relatório de Vendas Personalizado";
            
            // Simulando dados de vendas
            var totalVendas = random.Next(5000, 50000);
            var totalPedidos = random.Next(100, 1000);
            var ticketMedio = totalVendas / (totalPedidos > 0 ? totalPedidos : 1);
            
            report.Data.Add(new ReportDataDto { Key = "TotalVendas", Value = totalVendas.ToString("C") });
            report.Data.Add(new ReportDataDto { Key = "TotalPedidos", Value = totalPedidos.ToString() });
            report.Data.Add(new ReportDataDto { Key = "TicketMédio", Value = ticketMedio.ToString("C") });
            
            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue)
            {
                report.Data.Add(new ReportDataDto { Key = "PeriodoInicio", Value = parameters.StartDate.Value.ToShortDateString() });
                report.Data.Add(new ReportDataDto { Key = "PeriodoFim", Value = parameters.EndDate.Value.ToShortDateString() });
            }
        }

        private async Task GenerateStockReport(CustomReportDto report, ReportParametersDto parameters)
        {
            report.Title = "Relatório de Estoque Personalizado";
            
            var products = await _productRepository.GetProducts();
            var totalProdutos = products.Count();
            var totalEstoque = products.Sum(p => p.Stock);
            var produtosEstoqueBaixo = products.Count(p => p.Stock < 10);
            var valorTotalEstoque = products.Sum(p => p.Price * p.Stock);
            
            report.Data.Add(new ReportDataDto { Key = "TotalProdutos", Value = totalProdutos.ToString() });
            report.Data.Add(new ReportDataDto { Key = "TotalEstoque", Value = totalEstoque.ToString() });
            report.Data.Add(new ReportDataDto { Key = "ProdutosEstoqueBaixo", Value = produtosEstoqueBaixo.ToString() });
            report.Data.Add(new ReportDataDto { Key = "ValorTotalEstoque", Value = valorTotalEstoque.ToString("C") });
            
            if (parameters.CategoryId.HasValue)
            {
                var categoryProducts = products.Where(p => p.CategoryId == parameters.CategoryId.Value);
                var category = await _categoryRepository.GetById(parameters.CategoryId.Value);
                if (category != null)
                {
                    report.Data.Add(new ReportDataDto { Key = "Categoria", Value = category.Name });
                    report.Data.Add(new ReportDataDto { Key = "ProdutosNaCategoria", Value = categoryProducts.Count().ToString() });
                    report.Data.Add(new ReportDataDto { Key = "EstoqueNaCategoria", Value = categoryProducts.Sum(p => p.Stock).ToString() });
                }
            }
        }

        private async Task GenerateCategoryReport(CustomReportDto report, ReportParametersDto parameters)
        {
            report.Title = "Relatório de Categorias Personalizado";
            
            var categories = await _categoryRepository.GetCategories();
            var products = await _productRepository.GetProducts();
            
            var totalCategorias = categories.Count();
            
            report.Data.Add(new ReportDataDto { Key = "TotalCategorias", Value = totalCategorias.ToString() });
            
            if (parameters.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetById(parameters.CategoryId.Value);
                if (category != null)
                {
                    var categoryProducts = products.Where(p => p.CategoryId == category.Id);
                    report.Data.Add(new ReportDataDto { Key = "NomeCategoria", Value = category.Name });
                    report.Data.Add(new ReportDataDto { Key = "TotalProdutos", Value = categoryProducts.Count().ToString() });
                    report.Data.Add(new ReportDataDto { Key = "ValorTotalEstoque", Value = categoryProducts.Sum(p => p.Price * p.Stock).ToString("C") });
                }
            }
            else
            {
                // Adiciona informações resumidas de todas as categorias
                foreach (var category in categories.Take(5)) // Limita a 5 categorias para não sobrecarregar o relatório
                {
                    var categoryProducts = products.Where(p => p.CategoryId == category.Id);
                    report.Data.Add(new ReportDataDto { Key = $"Categoria_{category.Id}_Nome", Value = category.Name });
                    report.Data.Add(new ReportDataDto { Key = $"Categoria_{category.Id}_TotalProdutos", Value = categoryProducts.Count().ToString() });
                }
            }
        }

        private async Task GenerateProductReport(CustomReportDto report, ReportParametersDto parameters)
        {
            report.Title = "Relatório de Produto Personalizado";
            
            if (parameters.ProductId.HasValue)
            {
                var product = await _productRepository.GetById(parameters.ProductId.Value);
                if (product != null)
                {
                    var category = await _categoryRepository.GetById(product.CategoryId);
                    
                    report.Data.Add(new ReportDataDto { Key = "ProdutoId", Value = product.Id.ToString() });
                    report.Data.Add(new ReportDataDto { Key = "NomeProduto", Value = product.Name });
                    report.Data.Add(new ReportDataDto { Key = "Descrição", Value = product.Description });
                    report.Data.Add(new ReportDataDto { Key = "Preço", Value = product.Price.ToString("C") });
                    report.Data.Add(new ReportDataDto { Key = "Estoque", Value = product.Stock.ToString() });
                    report.Data.Add(new ReportDataDto { Key = "ValorTotalEstoque", Value = (product.Price * product.Stock).ToString("C") });
                    
                    if (category != null)
                    {
                        report.Data.Add(new ReportDataDto { Key = "Categoria", Value = category.Name });
                    }
                }
                else
                {
                    report.Data.Add(new ReportDataDto { Key = "Erro", Value = "Produto não encontrado" });
                }
            }
            else
            {
                var products = await _productRepository.GetProducts();
                var totalProdutos = products.Count();
                var precoMedio = products.Any() ? products.Average(p => p.Price) : 0;
                
                report.Data.Add(new ReportDataDto { Key = "TotalProdutos", Value = totalProdutos.ToString() });
                report.Data.Add(new ReportDataDto { Key = "PreçoMédio", Value = precoMedio.ToString("C") });
                
                // Adiciona informações dos 5 produtos mais caros
                var topProducts = products.OrderByDescending(p => p.Price).Take(5);
                int index = 1;
                foreach (var product in topProducts)
                {
                    report.Data.Add(new ReportDataDto { Key = $"TopProduto_{index}_Nome", Value = product.Name });
                    report.Data.Add(new ReportDataDto { Key = $"TopProduto_{index}_Preço", Value = product.Price.ToString("C") });
                    index++;
                }
            }
        }
    }
}