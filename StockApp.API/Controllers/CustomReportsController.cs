using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomReportsController : ControllerBase
    {
        private readonly ICustomReportService _customReportService;

        public CustomReportsController(ICustomReportService customReportService)
        {
            _customReportService = customReportService;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<CustomReportDto>> GenerateReport([FromBody] ReportParametersDto parameters)
        {
            if (parameters == null)
            {
                return BadRequest("Parâmetros de relatório inválidos");
            }

            var report = await _customReportService.GenerateReportAsync(parameters);
            return Ok(report);
        }

        [HttpGet("types")]
        public ActionResult<IEnumerable<string>> GetReportTypes()
        {
            // Retorna os tipos de relatórios disponíveis como strings simples
            var reportTypes = new List<string>
            {
                "vendas",
                "estoque",
                "categoria",
                "produto"
            };

            return Ok(reportTypes);
        }

        [HttpGet("parameters/{reportType}")]
        public ActionResult<object> GetReportParameters(string reportType)
        {
            // Retorna os parâmetros disponíveis para cada tipo de relatório
            var parameters = new List<object>();

            switch (reportType.ToLower())
            {
                case "vendas":
                    parameters.Add(new { Id = "startDate", Name = "Data Inicial", Type = "date" });
                    parameters.Add(new { Id = "endDate", Name = "Data Final", Type = "date" });
                    break;
                case "estoque":
                    parameters.Add(new { Id = "categoryId", Name = "Categoria", Type = "select" });
                    break;
                case "categoria":
                    parameters.Add(new { Id = "categoryId", Name = "Categoria", Type = "select" });
                    break;
                case "produto":
                    parameters.Add(new { Id = "productId", Name = "Produto", Type = "select" });
                    break;
                default:
                    return NotFound($"Tipo de relatório '{reportType}' não encontrado");
            }

            // Parâmetros comuns para todos os tipos de relatório
            parameters.Add(new { Id = "sortBy", Name = "Ordenar por", Type = "select" });
            parameters.Add(new { Id = "sortDirection", Name = "Direção", Type = "select", Options = new[] { "asc", "desc" } });

            return Ok(parameters);
        }
    }
}