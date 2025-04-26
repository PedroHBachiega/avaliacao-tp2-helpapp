using HelpApp.Domain.Entities;
using FluentAssertions;
using Xunit;
using HelpApp.Domain.Validation;

namespace HelpApp.Domain.Test
{
    public class CategoryUnitTest
    {
        #region Testes Positivos
        [Fact(DisplayName = "Create Category With Valid State")]
        public void CreateCategory_WithValidParameters_ResultObjectValidState()
        {
            Action action = () => new Category(1, "Category Name");
            action.Should().NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }

        [Fact(DisplayName = "Should Create Category With Name And Default id")]
        public void CreateCategory_WithNameAndDefaultId()
        {
            Action action = () => new Category("Category Name");
            action.Should().NotThrow<HelpApp.Domain.Validation.DomainExceptionValidation>();
        }
        
        [Fact(DisplayName = "Should Create Category And Set Properties Correctly")]
        public void CreateCategory_WithValidParameters_SetPropertiesCorrectly()
        {
            
            var category = new Category(1, "Category Name");
            
           
            category.Id.Should().Be(1);
            category.Name.Should().Be("Category Name");
        }
        
        [Fact(DisplayName = "Create Category With Long Name Should Be Valid")]
        public void CreateCategory_WithLongName_ShouldBeValid()
        {
            
            var longName = new string('a', 100);
            var category = new Category(1, longName);
            
            
            category.Name.Should().Be(longName);
        }

        [Fact(DisplayName = "Should Update Category Name Correctly")]
        public void UpdateName_WithValidName_ShouldUpdateNameSuccessfully()
        {
            
            var category = new Category(1, "Old Name");
            category.Update("New Name");
            category.Name.Should().Be("New Name");
        }
        
        #endregion
        
        #region Testes Negativos
        [Fact(DisplayName = "Create Category With Name Empty")]
        public void CreateCategory_WithNameEmpty_ResultObjetcException()
        {
            Action action = () => new Category(1, "");
            action.Should().Throw<HelpApp.Domain.Validation.DomainExceptionValidation>()
                .WithMessage("Invalid name, name is required.");
        }
        
        [Fact(DisplayName = "Create Category With Null Name Should Throw")]
        public void CreateCategory_WithNullName_ShouldThrowException()
        {
            Action action = () => new Category(1, null);
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, name is required.");
        }
        
        [Fact(DisplayName = "Create Category With Name Too Short Should Throw")]
        public void CreateCategory_WithNameTooShort_ShouldThrowException()
        {
            Action action = () => new Category(1, "ab");
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, too short, minimum 3 characters.");
        }
        
        [Fact(DisplayName = "Create Category With Negative Id Should Throw")]
        public void CreateCategory_WithNegativeId_ShouldThrowException()
        {
            Action action = () => new Category(-1, "Category Name");
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid Id value.");
        }

        [Fact(DisplayName = "Update Name With Empty Value Should Throw")]
        public void UpdateName_WithEmptyName_ShouldThrowException()
        {
            
            var category = new Category(1, "Category Name");
            Action action = () => category.Update("");
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, name is required.");
        }
        
        [Fact(DisplayName = "Update Name With Too Short Value Should Throw")]
        public void UpdateName_WithTooShortName_ShouldThrowException()
        {
            
            var category = new Category(1, "Category Name");
            
            Action action = () => category.Update("ab");
            action.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, too short, minimum 3 characters.");
        }
        
        #endregion
    }
}