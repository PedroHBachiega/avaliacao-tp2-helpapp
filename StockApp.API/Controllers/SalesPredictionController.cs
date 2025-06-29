using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesPredictionController : ControllerBase
    {
        private readonly ISalesPredictionService _salesPredictionService;

        public SalesPredictionController(ISalesPredictionService salesPredictionService)
        {
            _salesPredictionService = salesPredictionService;
        }

        /// <summary>
        /// Obtém a previsão de vendas para um produto específico em um determinado mês e ano
        /// </summary>
        /// <param name="productId">ID do produto</param>
        /// <param name="month">Mês (1-12)</param>
        /// <param name="year">Ano</param>
        /// <returns>Previsão de vendas</returns>
        [HttpGet("{productId}/{month}/{year}")]
        [Authorize(Policy = "CanManageStock")]
        [ProducesResponseType(typeof(SalesPredictionDTO), 200)]
        public IActionResult GetSalesPrediction(int productId, int month, int year)
        {
            // Validar parâmetros
            if (month < 1 || month > 12)
            {
                return BadRequest("O mês deve estar entre 1 e 12.");
            }

            if (year < DateTime.Now.Year)
            {
                return BadRequest("O ano deve ser o atual ou futuro.");
            }

            // Obter previsão
            double predictedSales = _salesPredictionService.PredictSales(productId, month, year);

            // Criar DTO de resposta
            var predictionDto = new SalesPredictionDTO
            {
                ProductId = productId,
                Month = month,
                Year = year,
                PredictedSales = predictedSales
            };

            return Ok(predictionDto);
        }
    }
}