using HelpApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace HelpApp.Domain.Test
{
    public class ProductUnitTest
    {
        #region Testes Positivos
        [Fact(DisplayName ="Create Product With Parameters Full")]
        public void CreateProduct_WithValidParameters_ResultObjectValisState()
        {
            Action action= () => new Product(1, 
                "Product Name", 
                "Product Description", 
                9.99m, 99, 
                "https://img/product.jpg");
            action.Should()
                .NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }
        
        [Fact(DisplayName = "Create Product With Constructor Without Id")]
        public void CreateProduct_WithoutIdParameter_ResultObjectValidState()
        {
            Action action = () => new Product(
                "Product Name",
                "Product Description",
                9.99m, 99,
                "https://img/product.jpg");
            action.Should()
                .NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }
        
        [Fact(DisplayName = "Create Product And Set Properties Correctly")]
        public void CreateProduct_WithValidParameters_SetPropertiesCorrectly()
        {
            
            var product = new Product(1, 
                "Product Name", 
                "Product Description", 
                9.99m, 99, 
                "https://img/product.jpg");
            
            
            product.Id.Should().Be(1);
            product.Name.Should().Be("Product Name");
            product.Description.Should().Be("Product Description");
            product.Price.Should().Be(9.99m);
            product.Stock.Should().Be(99);
            product.Image.Should().Be("https://img/product.jpg");
        }
        
        [Fact(DisplayName = "Update Product Method Should Change Properties")]
        public void UpdateProduct_WithValidParameters_PropertiesShouldChange()
        {
           
            var product = new Product(1, 
                "Product Name", 
                "Product Description", 
                9.99m, 99, 
                "https://img/product.jpg");
                
           
            product.Update(
                "New Product Name",
                "New Product Description",
                19.99m, 50,
                "https://img/new-product.jpg",
                2);
                
           
            product.Name.Should().Be("New Product Name");
            product.Description.Should().Be("New Product Description");
            product.Price.Should().Be(19.99m);
            product.Stock.Should().Be(50);
            product.Image.Should().Be("https://img/new-product.jpg");
            product.CategoryId.Should().Be(2);
        }
        
        [Fact(DisplayName = "Update Category Id Only")]
        public void UpdateCategoryId_WithValidId_ShouldChangeCategoryId()
        {
            
            var product = new Product(1, 
                "Product Name", 
                "Product Description", 
                9.99m, 99, 
                "https://img/product.jpg");
                
            
            product.UpdateCategory(5);
                
            
            product.CategoryId.Should().Be(5);
        }
        #endregion
        
        #region Testes Negativos
        [Fact(DisplayName ="Create Product With ID Negative")]
        public void CreateProduct_NegativeIdValue_DomainExceptionInvalidId()
        {
            Action action = () => new Product(-1, "Product Name", "Product Description", 9.99m,
                99, "product image");

            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Update Invalid Id value");
        }

        [Fact(DisplayName = "Create Product With Short Name")]
        public void CreateProduct_ShortNameValue_DomainExceptionShortName()
        {
            Action action = () => new Product(1, "Pr", "Product Description", 9.99m, 99,
                "product image");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid name, too short, minimum 3 characters.");
        }

        [Fact(DisplayName = "Create Product With Null URL Image")]
        public void CreateProduct_WithNullImageName_NoDomainException()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, null);
            action.Should().NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }


        [Fact(DisplayName = "Create Product With URL Image Empty")]
        public void CreateProduct_WithEmptyImageName_NoDomainException()
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m, 99, "");
            action.Should().NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }

        [Theory(DisplayName = "Create Product With Invalid Price")]
        [InlineData(-25)]
        public void CreateProduct_InvalidPriceValue_DomainException(int value)
        {
            Action action = () => new Product(1, "Product Name", "Product Description", value,
                99, "");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid price negative value.");
        }

        [Theory(DisplayName = "Create Product With Inavlid Stock")]
        [InlineData(-5)]
        public void CreateProduct_InvalidStockValue_ExceptionDomainNegativeValue(int value)
        {
            Action action = () => new Product(1, "Pro", "Product Description", 9.99m, value,
                "product image");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                   .WithMessage("Invalid stock negative value.");
        }
        
        [Theory(DisplayName = "Create Product With Long URL Image")]
        [InlineData("https://avatars.githubusercontent.com/u/654654651398798798798798798798798798654654321321321321321321321321321321321321321321321658479879879846465465465465465132198498498465465246549879879846213219849849846521321684684987465132165419687498746541631658468465840123321005408?v=4&size=64")]
        public void CreateProduct_LongImageName_DomainExceptionLongImageName(string url)
        {
            Action action = () => new Product(1, "Product Name", "Product Description", 9.99m,
                99, url);
            action.Should()
                .Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid image name, too long, maximum 250 characters.");
        }
        
        [Fact(DisplayName = "Create Product With Empty Description")]
        public void CreateProduct_EmptyDescription_DomainExceptionEmptyDescription()
        {
            Action action = () => new Product(1, "Product Name", "", 9.99m, 99, "product image");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid description, name is required.");
        }
        
        [Fact(DisplayName = "Create Product With Short Description")]
        public void CreateProduct_ShortDescription_DomainExceptionShortDescription()
        {
            Action action = () => new Product(1, "Product Name", "Desc", 9.99m, 99, "product image");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid description, too short, minimum 5 characters.");
        }
        
        [Fact(DisplayName = "Update Product With Invalid Name")]
        public void UpdateProduct_WithInvalidName_ShouldThrowException()
        {
            
            var product = new Product(1, "Product Name", "Product Description", 9.99m, 99, "image.jpg");
            
            
            Action action = () => product.Update("", "New Description", 19.99m, 50, "new-image.jpg", 2);
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid name, name is required.");
        }
        
        [Fact(DisplayName = "Update Category With Invalid Id")]
        public void UpdateCategory_WithNegativeId_ShouldThrowException()
        {
            
            var product = new Product(1, "Product Name", "Product Description", 9.99m, 99, "image.jpg");
            
            
            Action action = () => product.UpdateCategory(-1);
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                 .WithMessage("Invalid category Id value");
        }
        #endregion
    }
}