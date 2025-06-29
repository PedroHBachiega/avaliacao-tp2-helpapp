using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class EmployeePerformanceEvaluationService : IEmployeePerformanceEvaluationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeEvaluationRepository _evaluationRepository;
        private readonly IMapper _mapper;

        public EmployeePerformanceEvaluationService(IEmployeeRepository employeeRepository, 
                                                  IEmployeeEvaluationRepository evaluationRepository,
                                                  IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _evaluationRepository = evaluationRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeEvaluationDto> EvaluatePerformanceAsync(int employeeId)
        {
            // Verificar se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new ApplicationException($"Funcionário com ID {employeeId} não encontrado.");

            // Criar uma avaliação padrão para demonstração
            return new EmployeeEvaluationDto
            {
                EmployeeId = employeeId,
                EvaluationScore = 85,
                Feedback = "Excelente desempenho",
                Goals = "Continuar melhorando as habilidades técnicas e de comunicação",
                EvaluationDate = DateTime.Now,
                EvaluatedBy = "Sistema"
            };
        }

        public async Task<EmployeeEvaluationDto> CreateEvaluationAsync(CreateEmployeeEvaluationDto evaluationDto)
        {
            // Verificar se o funcionário existe
            var employee = await _employeeRepository.GetByIdAsync(evaluationDto.EmployeeId);
            if (employee == null)
                throw new ApplicationException($"Funcionário com ID {evaluationDto.EmployeeId} não encontrado.");

            // Criar a entidade de avaliação
            var evaluation = new EmployeeEvaluation(
                evaluationDto.EmployeeId,
                evaluationDto.EvaluationScore,
                evaluationDto.Feedback,
                evaluationDto.Goals,
                evaluationDto.EvaluationDate,
                evaluationDto.EvaluatedBy);

            // Salvar no repositório
            evaluation = await _evaluationRepository.CreateAsync(evaluation);

            // Mapear para DTO e retornar
            var result = _mapper.Map<EmployeeEvaluationDto>(evaluation);
            result.Employee = _mapper.Map<EmployeeDTO>(employee);
            return result;
        }

        public async Task<EmployeeEvaluationDto> GetEvaluationByIdAsync(int id)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
                throw new ApplicationException($"Avaliação com ID {id} não encontrada.");

            var employee = await _employeeRepository.GetByIdAsync(evaluation.EmployeeId);
            
            var evaluationDto = _mapper.Map<EmployeeEvaluationDto>(evaluation);
            evaluationDto.Employee = _mapper.Map<EmployeeDTO>(employee);
            
            return evaluationDto;
        }

        public async Task<IEnumerable<EmployeeEvaluationDto>> GetAllEvaluationsAsync()
        {
            var evaluations = await _evaluationRepository.GetEvaluationsAsync();
            var evaluationDtos = new List<EmployeeEvaluationDto>();

            foreach (var evaluation in evaluations)
            {
                var employee = await _employeeRepository.GetByIdAsync(evaluation.EmployeeId);
                var evaluationDto = _mapper.Map<EmployeeEvaluationDto>(evaluation);
                evaluationDto.Employee = _mapper.Map<EmployeeDTO>(employee);
                evaluationDtos.Add(evaluationDto);
            }

            return evaluationDtos;
        }

        public async Task<IEnumerable<EmployeeEvaluationDto>> GetEvaluationsByEmployeeIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new ApplicationException($"Funcionário com ID {employeeId} não encontrado.");

            var evaluations = await _evaluationRepository.GetEvaluationsByEmployeeIdAsync(employeeId);
            var evaluationDtos = new List<EmployeeEvaluationDto>();

            foreach (var evaluation in evaluations)
            {
                var evaluationDto = _mapper.Map<EmployeeEvaluationDto>(evaluation);
                evaluationDto.Employee = _mapper.Map<EmployeeDTO>(employee);
                evaluationDtos.Add(evaluationDto);
            }

            return evaluationDtos;
        }

        public async Task UpdateEvaluationAsync(EmployeeEvaluationDto evaluationDto)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(evaluationDto.Id);
            if (evaluation == null)
                throw new ApplicationException($"Avaliação com ID {evaluationDto.Id} não encontrada.");

            evaluation.Update(
                evaluationDto.EvaluationScore,
                evaluationDto.Feedback,
                evaluationDto.Goals,
                evaluationDto.EvaluatedBy);

            await _evaluationRepository.UpdateAsync(evaluation);
        }

        public async Task RemoveEvaluationAsync(int id)
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(id);
            if (evaluation == null)
                throw new ApplicationException($"Avaliação com ID {id} não encontrada.");

            await _evaluationRepository.RemoveAsync(evaluation);
        }
    }
}