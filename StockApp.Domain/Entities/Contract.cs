using StockApp.Domain.Validation;
using System;

namespace StockApp.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um contrato
    /// </summary>
    public class Contract
    {
        public int Id { get; private set; }
        public int? SupplierId { get; private set; }
        public int? ClientId { get; private set; }
        public string ContractNumber { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal Value { get; private set; }
        public string Terms { get; private set; }
        public bool IsActive { get; private set; }
        public int ContractType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Propriedades de navegação
        public Supplier Supplier { get; set; }
        
        public Contract(string contractNumber, string description, DateTime startDate, DateTime endDate, decimal value, string terms, int contractType)
        {
            ValidateDomain(contractNumber, description, startDate, endDate, value, terms, contractType);
            IsActive = true;
            CreatedAt = DateTime.Now;
        }
        
        public Contract(int id, int? supplierId, int? clientId, string contractNumber, string description, 
            DateTime startDate, DateTime endDate, decimal value, string terms, bool isActive, int contractType)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            DomainExceptionValidation.When(supplierId.HasValue && supplierId.Value < 0, "Invalid SupplierId value.");
            DomainExceptionValidation.When(clientId.HasValue && clientId.Value < 0, "Invalid ClientId value.");
            DomainExceptionValidation.When(!supplierId.HasValue && !clientId.HasValue, "Either SupplierId or ClientId must be provided.");
            
            Id = id;
            SupplierId = supplierId;
            ClientId = clientId;
            ValidateDomain(contractNumber, description, startDate, endDate, value, terms, contractType);
            IsActive = isActive;
            CreatedAt = DateTime.Now;
        }
        
        public void Update(string description, DateTime endDate, decimal value, string terms, bool isActive)
        {
            ValidateUpdate(description, endDate, value, terms);
            IsActive = isActive;
            UpdatedAt = DateTime.Now;
        }
        
        public void Renew(DateTime newEndDate)
        {
            DomainExceptionValidation.When(StartDate > newEndDate, "New end date must be after start date.");
            DomainExceptionValidation.When(EndDate >= newEndDate, "New end date must be after current end date.");
            
            EndDate = newEndDate;
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }
        
        private void ValidateDomain(string contractNumber, string description, DateTime startDate, DateTime endDate, decimal value, string terms, int contractType)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(contractNumber), "Contract number is required.");
            DomainExceptionValidation.When(contractNumber.Length < 3, "Contract number too short. Minimum 3 characters.");
            
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(description), "Description is required.");
            DomainExceptionValidation.When(description.Length < 10, "Description too short. Minimum 10 characters.");
            
            DomainExceptionValidation.When(startDate > endDate, "Start date must be before end date.");
            
            DomainExceptionValidation.When(value < 0, "Value must be greater than or equal to zero.");
            
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(terms), "Terms are required.");
            
            DomainExceptionValidation.When(contractType < 1 || contractType > 2, "Invalid contract type.");
            
            ContractNumber = contractNumber;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Value = value;
            Terms = terms;
            ContractType = contractType;
        }
        
        private void ValidateUpdate(string description, DateTime endDate, decimal value, string terms)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(description), "Description is required.");
            DomainExceptionValidation.When(description.Length < 10, "Description too short. Minimum 10 characters.");
            
            DomainExceptionValidation.When(StartDate > endDate, "End date must be after start date.");
            
            DomainExceptionValidation.When(value < 0, "Value must be greater than or equal to zero.");
            
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(terms), "Terms are required.");
            
            Description = description;
            EndDate = endDate;
            Value = value;
            Terms = terms;
        }
    }
}