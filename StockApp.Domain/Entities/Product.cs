using StockApp.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Domain.Entities
{
    public class Product
    {
        #region Atributos
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int MinimumStockLevel { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? PromotionDescription { get; set; }

        #endregion

        public Product(string name, string description, decimal price, int stock, string image)
        {
            ValidateDomain(name, description, price, stock, image);
        }

        public Product(int id, string name, string description, decimal price, int stock, string image)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(name, description, price, stock, image);
        }

        public Product(int id, string name, string description, decimal price, int stock, string image, int categoryId)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(name, description, price, stock, image, categoryId);
        }



        public Category Category { get; set; }

        #region Métodos de Atualização
        public void Update(string name, string description, decimal price, int stock, string image)
        {
            ValidateDomain(name, description, price, stock, image);
        }

        public void Update(string name, string description, decimal price, int stock, string image, int categoryId)
        {
            ValidateDomain(name, description, price, stock, image, categoryId);
        }

        public void UpdateStock(int newStock)
        {
            DomainExceptionValidation.When(newStock < 0, "Invalid stock, negative value not allowed.");
            DomainExceptionValidation.When(newStock > 999999, "Invalid stock, maximum value is 999999.");
            Stock = newStock;
        }

        public void UpdatePrice(decimal newPrice)
        {
            DomainExceptionValidation.When(newPrice < 0, "Invalid price, negative value not allowed.");
            DomainExceptionValidation.When(newPrice > 999999.99m, "Invalid price, maximum value is 999999.99.");
            Price = newPrice;
        }

        public void UpdateCategory(int categoryId)
        {
            DomainExceptionValidation.When(categoryId <= 0, "Invalid CategoryId, must be greater than zero.");
            CategoryId = categoryId;
        }
        #endregion

        private void ValidateDomain(string name, string description, decimal price, int stock, string image, int? categoryId = null)
        {
            // Validação do Nome
            DomainExceptionValidation.When(string.IsNullOrEmpty(name),
                "Invalid name, name is required.");

            DomainExceptionValidation.When(name.Length < 3,
                "Invalid name, too short, minimum 3 characters.");

            DomainExceptionValidation.When(name.Length > 100,
                "Invalid name, too long, maximum 100 characters.");

            // Validação da Descrição
            DomainExceptionValidation.When(string.IsNullOrEmpty(description),
                "Invalid description, description is required.");

            DomainExceptionValidation.When(description.Length < 5,
                "Invalid description, too short, minimum 5 characters.");

            DomainExceptionValidation.When(description.Length > 500,
                "Invalid description, too long, maximum 500 characters.");

            // Validação do Preço
            DomainExceptionValidation.When(price < 0, "Invalid price, negative value not allowed.");

            DomainExceptionValidation.When(price > 999999.99m, "Invalid price, maximum value is 999999.99.");

            // Validação do Estoque
            DomainExceptionValidation.When(stock < 0, "Invalid stock, negative value not allowed.");

            DomainExceptionValidation.When(stock > 999999, "Invalid stock, maximum value is 999999.");

            // Validação da Imagem
            DomainExceptionValidation.When(!string.IsNullOrEmpty(image) && image.Length > 250,
                "Invalid image name, too long, maximum 250 characters.");

            DomainExceptionValidation.When(MinimumStockLevel < 0, "Invalid MinimumStockLevel, negative value not allowed.");

            DomainExceptionValidation.When(MinimumStockLevel > 999999, "Invalid MinimumStockLevel, maximum value is 999999.");

            // Validação da Categoria
            if (categoryId.HasValue)
            {
                DomainExceptionValidation.When(categoryId.Value <= 0, "Invalid CategoryId, must be greater than zero.");

                CategoryId = categoryId.Value;
            }

            // Atribuição dos valores após validação
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Image = image;
            MinimumStockLevel = 10; 
        }
    }
}