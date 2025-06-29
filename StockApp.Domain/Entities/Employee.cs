using StockApp.Domain.Validation;
using System;

namespace StockApp.Domain.Entities
{
    public class Employee
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Position { get; private set; }
        public string Department { get; private set; }
        public DateTime HireDate { get; private set; }

        public Employee(string name, string position, string department, DateTime hireDate)
        {
            ValidateDomain(name, position, department, hireDate);
        }

        public Employee(int id, string name, string position, string department, DateTime hireDate)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(name, position, department, hireDate);
        }

        public void Update(string name, string position, string department)
        {
            ValidateDomain(name, position, department, this.HireDate);
        }

        private void ValidateDomain(string name, string position, string department, DateTime hireDate)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(name),
                "Invalid name. Name is required");

            DomainExceptionValidation.When(name.Length < 3,
                "Invalid name. Too short, minimum 3 characters");

            DomainExceptionValidation.When(string.IsNullOrEmpty(position),
                "Invalid position. Position is required");

            DomainExceptionValidation.When(string.IsNullOrEmpty(department),
                "Invalid department. Department is required");

            DomainExceptionValidation.When(hireDate > DateTime.Now,
                "Invalid hire date. Hire date cannot be in the future");

            Name = name;
            Position = position;
            Department = department;
            HireDate = hireDate;
        }
    }
}