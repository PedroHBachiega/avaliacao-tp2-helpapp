using StockApp.Domain.Validation;
using System.Collections.Generic;

namespace StockApp.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um fornecedor
    /// </summary>
    public class Supplier
    {
        #region Atributos
        public int Id { get; set; }

        public string Name { get; set; }
        public string ContactEmail { get; set; }

        public string PhoneNumber { get; set; }
        
        // Coleções de navegação
        public ICollection<SupplierContract> Contracts { get; set; } = new List<SupplierContract>();
        public ICollection<SupplierEvaluation> Evaluations { get; set; } = new List<SupplierEvaluation>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        #endregion

        #region Construtores
        public Supplier(string name, string contactEmail, string phoneNumber)
        {

            ValidateDomain(name, contactEmail, phoneNumber);

        }

        public Supplier(int id, string name, string contactEmail, string phoneNumber)
        {

            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;

            ValidateDomain(name, contactEmail, phoneNumber);

        }
        #endregion

        #region Validação
        private void ValidateDomain(string name, string contactEmail, string phoneNumber)
        {
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(name), "Name is required.");

            DomainExceptionValidation.When(name.Length < 3, "Name too short. Minimum 3 characters.");


            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(contactEmail), "Contact email is required.");
            DomainExceptionValidation.When(!contactEmail.Contains("@"), "Invalid email format.");


            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(phoneNumber), "Phone number is required.");
            DomainExceptionValidation.When(phoneNumber.Length < 8, "Phone number too short.");

            Name = name;

            ContactEmail = contactEmail;

            PhoneNumber = phoneNumber;
        }
        #endregion
    }
}
