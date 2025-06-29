using System;
using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    /// <summary>
    /// DTO para contrato
    /// </summary>
    public class ContractDto
    {
        /// <summary>
        /// ID do contrato
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID do fornecedor (opcional, pode ser nulo se for contrato de cliente)
        /// </summary>
        public int? SupplierId { get; set; }
        
        /// <summary>
        /// ID do cliente (opcional, pode ser nulo se for contrato de fornecedor)
        /// </summary>
        public int? ClientId { get; set; }
        
        /// <summary>
        /// Número do contrato
        /// </summary>
        [Required(ErrorMessage = "O Número do contrato é obrigatório.")]
        [MinLength(3, ErrorMessage = "O Número do contrato deve ter pelo menos 3 caracteres.")]
        public string ContractNumber { get; set; }
        
        /// <summary>
        /// Descrição do contrato
        /// </summary>
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [MinLength(10, ErrorMessage = "A Descrição deve ter pelo menos 10 caracteres.")]
        public string Description { get; set; }
        
        /// <summary>
        /// Data de início do contrato
        /// </summary>
        [Required(ErrorMessage = "A Data de início é obrigatória.")]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Data de término do contrato
        /// </summary>
        [Required(ErrorMessage = "A Data de término é obrigatória.")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Valor do contrato
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "O Valor deve ser maior ou igual a zero.")]
        public decimal Value { get; set; }
        
        /// <summary>
        /// Termos do contrato
        /// </summary>
        [Required(ErrorMessage = "Os Termos do contrato são obrigatórios.")]
        public string Terms { get; set; }
        
        /// <summary>
        /// Indica se o contrato está ativo
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Tipo do contrato (Fornecedor ou Cliente)
        /// </summary>
        [Required(ErrorMessage = "O Tipo do contrato é obrigatório.")]
        public ContractType ContractType { get; set; }
        
        /// <summary>
        /// Data de criação do contrato
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Data de atualização do contrato
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Dias restantes até a expiração do contrato
        /// </summary>
        public int DaysUntilExpiration => (EndDate - DateTime.Now).Days;
        
        /// <summary>
        /// Indica se o contrato está próximo da expiração (menos de 30 dias)
        /// </summary>
        public bool IsNearExpiration => DaysUntilExpiration <= 30 && DaysUntilExpiration >= 0;
        
        /// <summary>
        /// Indica se o contrato está expirado
        /// </summary>
        public bool IsExpired => DateTime.Now > EndDate;
    }
}