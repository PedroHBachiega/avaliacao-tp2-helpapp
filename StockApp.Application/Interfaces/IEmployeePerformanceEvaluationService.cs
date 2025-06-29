using StockApp.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IEmployeePerformanceEvaluationService
    {
        Task<EmployeeEvaluationDto> EvaluatePerformanceAsync(int employeeId);
        Task<EmployeeEvaluationDto> CreateEvaluationAsync(CreateEmployeeEvaluationDto evaluationDto);
        Task<EmployeeEvaluationDto> GetEvaluationByIdAsync(int id);
        Task<IEnumerable<EmployeeEvaluationDto>> GetAllEvaluationsAsync();
        Task<IEnumerable<EmployeeEvaluationDto>> GetEvaluationsByEmployeeIdAsync(int employeeId);
        Task UpdateEvaluationAsync(EmployeeEvaluationDto evaluationDto);
        Task RemoveEvaluationAsync(int id);
    }
}