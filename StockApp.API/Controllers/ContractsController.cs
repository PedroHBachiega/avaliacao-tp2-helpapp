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
    /// Controlador para gestão de contratos
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractManagementService _contractManagementService;

        public ContractsController(IContractManagementService contractManagementService)
        {
            _contractManagementService = contractManagementService;
        }

        /// <summary>
        /// Obtém todos os contratos
        /// </summary>
        /// <returns>Lista de contratos</returns>
        /// <response code="200">Contratos obtidos com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAllContracts()
        {
            var result = await _contractManagementService.GetAllContractsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtém um contrato pelo ID
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <returns>Dados do contrato</returns>
        /// <response code="200">Contrato obtido com sucesso</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Contrato não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContractDto>> GetContractById(int id)
        {
            try
            {
                var result = await _contractManagementService.GetContractByIdAsync(id);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona um novo contrato
        /// </summary>
        /// <param name="createContractDto">Dados do contrato a ser criado</param>
        /// <returns>Dados do contrato criado</returns>
        /// <response code="201">Contrato criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Fornecedor ou cliente não encontrado</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContractDto>> AddContract([FromBody] CreateContractDto createContractDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _contractManagementService.AddContractAsync(createContractDto);
                return CreatedAtAction(nameof(GetContractById), new { id = result.Id }, result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um contrato existente
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <param name="contractDto">Dados do contrato a ser atualizado</param>
        /// <returns>Dados do contrato atualizado</returns>
        /// <response code="200">Contrato atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Contrato não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContractDto>> UpdateContract(int id, [FromBody] ContractDto contractDto)
        {
            if (id != contractDto.Id)
                return BadRequest("ID do contrato não corresponde ao ID da rota.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _contractManagementService.UpdateContractAsync(contractDto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Remove um contrato
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Contrato removido com sucesso</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Contrato não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> RemoveContract(int id)
        {
            var result = await _contractManagementService.RemoveContractAsync(id);
            if (!result)
                return NotFound($"Contrato com ID {id} não encontrado.");

            return Ok($"Contrato com ID {id} removido com sucesso.");
        }

        /// <summary>
        /// Renova um contrato existente
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <param name="newEndDate">Nova data de término</param>
        /// <returns>Dados do contrato renovado</returns>
        /// <response code="200">Contrato renovado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Contrato não encontrado</response>
        [HttpPatch("{id}/renew")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ContractDto>> RenewContract(int id, [FromBody] DateTime newEndDate)
        {
            try
            {
                var result = await _contractManagementService.RenewContractAsync(id, newEndDate);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Obtém contratos próximos da expiração
        /// </summary>
        /// <param name="daysThreshold">Limite de dias para expiração</param>
        /// <returns>Lista de contratos próximos da expiração</returns>
        /// <response code="200">Contratos obtidos com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("near-expiration/{daysThreshold}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsNearExpiration(int daysThreshold)
        {
            var result = await _contractManagementService.GetContractsNearExpirationAsync(daysThreshold);
            return Ok(result);
        }
    }
}