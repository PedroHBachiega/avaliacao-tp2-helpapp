using System;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class EmployeeEvaluationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do Funcionário é obrigatório.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "A Pontuação da Avaliação é obrigatória.")]
        [Range(0, 100, ErrorMessage = "A Pontuação deve estar entre 0 e 100.")]
        public int EvaluationScore { get; set; }

        [Required(ErrorMessage = "O Feedback é obrigatório.")]
        public string Feedback { get; set; }

        [Required(ErrorMessage = "As Metas são obrigatórias.")]
        public string Goals { get; set; }

        public DateTime EvaluationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O Avaliador é obrigatório.")]
        public string EvaluatedBy { get; set; }

        // Propriedade de navegação para exibir informações do funcionário
        public EmployeeDTO Employee { get; set; }
    }
}