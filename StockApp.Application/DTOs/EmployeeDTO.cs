using System;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter pelo menos 3 caracteres.")]
        [MaxLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Cargo é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O Cargo deve ter no máximo 50 caracteres.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "O Departamento é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O Departamento deve ter no máximo 50 caracteres.")]
        public string Department { get; set; }

        [Required(ErrorMessage = "A Data de Contratação é obrigatória.")]
        public DateTime HireDate { get; set; }
    }
}