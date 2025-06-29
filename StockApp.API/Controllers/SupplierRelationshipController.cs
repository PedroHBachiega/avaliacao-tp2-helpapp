using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    /// <summary>
    /// Controlador para gestão de relacionamento com fornecedores
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierRelationshipController : ControllerBase
    {
        private readonly ISupplierRelationshipManagementService _relationshipService;

        public SupplierRelationshipController(ISupplierRelationshipManagementService relationshipService)
        {
            _relationshipService = relationshipService;
        }

        /// <summary>
        /// Avalia um fornecedor
        /// </summary>
        /// <param name="supplierId">ID do fornecedor</param>
        /// <returns>Dados do fornecedor avaliado</returns>
        [HttpGet("suppliers/{supplierId}/evaluate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<SupplierDTO>> EvaluateSupplier(int supplierId)
        {
            try
            {
                var result = await _relationshipService.EvaluateSupplierAsync(supplierId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona uma nova avaliação para um fornecedor
        /// </summary>
        /// <param name="evaluationDto">Dados da avaliação</param>
        /// <returns>Dados da avaliação criada</returns>
        [HttpPost("suppliers/evaluations")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<SupplierEvaluationDto>> AddEvaluation([FromBody] SupplierEvaluationDto evaluationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _relationshipService.AddEvaluationAsync(evaluationDto);
                return CreatedAtAction(nameof(GetSupplierEvaluations), new { supplierId = result.SupplierId }, result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas as avaliações de um fornecedor
        /// </summary>
        /// <param name="supplierId">ID do fornecedor</param>
        /// <returns>Lista de avaliações do fornecedor</returns>
        [HttpGet("suppliers/{supplierId}/evaluations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<SupplierEvaluationDto>>> GetSupplierEvaluations(int supplierId)
        {
            try
            {
                var result = await _relationshipService.GetSupplierEvaluationsAsync(supplierId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Renova um contrato de fornecedor
        /// </summary>
        /// <param name="contractId">ID do contrato</param>
        /// <param name="newEndDate">Nova data de término</param>
        /// <returns>Dados do contrato renovado</returns>
        [HttpPut("contracts/{contractId}/renew")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<SupplierContractDto>> RenewContract(int contractId, [FromBody] DateTime newEndDate)
        {
            try
            {
                var result = await _relationshipService.RenewContractAsync(contractId, newEndDate);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Verifica contratos próximos do vencimento
        /// </summary>
        /// <param name="daysThreshold">Limite de dias para considerar próximo do vencimento</param>
        /// <returns>Lista de contratos próximos do vencimento</returns>
        [HttpGet("contracts/near-expiration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<SupplierContractDto>>> GetContractsNearExpiration([FromQuery] int daysThreshold = 30)
        {
            var result = await _relationshipService.GetContractsNearExpirationAsync(daysThreshold);
            return Ok(result);
        }
    }
}