using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeEvaluationController : ControllerBase
    {
        private readonly IEmployeePerformanceEvaluationService _evaluationService;

        public EmployeeEvaluationController(IEmployeePerformanceEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDto>>> Get()
        {
            var evaluations = await _evaluationService.GetAllEvaluationsAsync();
            return Ok(evaluations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeEvaluationDto>> Get(int id)
        {
            try
            {
                var evaluation = await _evaluationService.GetEvaluationByIdAsync(id);
                return Ok(evaluation);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeEvaluationDto>>> GetByEmployeeId(int employeeId)
        {
            try
            {
                var evaluations = await _evaluationService.GetEvaluationsByEmployeeIdAsync(employeeId);
                return Ok(evaluations);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeEvaluationDto>> Post([FromBody] CreateEmployeeEvaluationDto evaluationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _evaluationService.CreateEvaluationAsync(evaluationDto);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EmployeeEvaluationDto evaluationDto)
        {
            if (id != evaluationDto.Id)
            {
                return BadRequest("IDs não correspondem");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _evaluationService.UpdateEvaluationAsync(evaluationDto);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _evaluationService.RemoveEvaluationAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("evaluate/{employeeId}")]
        public async Task<ActionResult<EmployeeEvaluationDto>> EvaluatePerformance(int employeeId)
        {
            try
            {
                var evaluation = await _evaluationService.EvaluatePerformanceAsync(employeeId);
                return Ok(evaluation);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}