using HelpApp.Domain.Entities;
using HelpApp.Domain.Validation;

namespace HelpApp.Domain.Entities
{
    public class Product
    {
        #region Atributos
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        #endregion
        
        
        private Product (){}
        public Product(string name, string description, decimal price, int stock, string image)
        {
            ValidateDomain(name, description, price, stock, image);
        }

        public Product(int id, string name, string description, decimal price, int stock, string image)
        {
            DomainExceptionValidation.When(id < 0, "Update Invalid Id value");
            Id = id;
            ValidateDomain(name, description, price, stock, image);
        }
        
        public void Update(string name, string description, decimal price, int stock, string image, int categoryId)
        {
            ValidateDomain(name, description, price, stock, image);
            CategoryId = categoryId;
        }

        public void UpdateCategory(int categoryId)
        {
            DomainExceptionValidation.When(categoryId <= 0, "Invalid category Id value");
            CategoryId = categoryId;
        }
        
        public Category Category { get; set; }

        private void ValidateDomain(string name, string description, decimal price, int stock, string image)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name),
                "Invalid name, name is required.");

            DomainExceptionValidation.When(name.Length < 3,
                "Invalid name, too short, minimum 3 characters.");

            DomainExceptionValidation.When(string.IsNullOrEmpty(description),
                "Invalid description, name is required.");

            DomainExceptionValidation.When(description.Length < 5,
                "Invalid description, too short, minimum 5 characters.");

            DomainExceptionValidation.When(price < 0, "Invalid price negative value.");

            DomainExceptionValidation.When(stock < 0, "Invalid stock negative value.");

            DomainExceptionValidation.When(  !String.IsNullOrEmpty(image)&&image.Length > 250, "Invalid image name, too long, maximum 250 characters.");
            
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Image = image;

            
        }
    }
}