using HelpApp.Domain.Validation;

namespace HelpApp.Domain.Entities
{
    public class Category
    {
        #region Atributos
        public int Id { get; set; }
        public string? Name { get; set; }
        #endregion

        #region Construtores

        private Category()
        {
            Products = new List<Product>();
        }
        public Category(string name)
        {
            ValidateDomain(name);
            Products = new List<Product>();
        }

        public Category(int id, string name)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(name);
            Products = new List<Product>();
        }

        public ICollection<Product> Products { get; set; }
        #endregion
        
        #region Métodos
        public void Update(string name)
        {
            ValidateDomain(name);
        }
        #endregion


        #region Validação
        private void ValidateDomain(string name)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name),
                "Invalid name, name is required.");

            DomainExceptionValidation.When(name.Length < 3,
                "Invalid name, too short, minimum 3 characters.");

            Name = name;
        }
        #endregion
    }
}
