using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetByIdAsync(int? id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> RemoveAsync(Employee employee);
    }
}