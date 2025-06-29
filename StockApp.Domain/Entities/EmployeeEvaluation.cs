using StockApp.Domain.Validation;
using System;

namespace StockApp.Domain.Entities
{
    public class EmployeeEvaluation
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public Employee Employee { get; set; }
        public int EvaluationScore { get; private set; }
        public string Feedback { get; private set; }
        public string Goals { get; private set; }
        public DateTime EvaluationDate { get; private set; }
        public string EvaluatedBy { get; private set; }

        public EmployeeEvaluation(int employeeId, int evaluationScore, string feedback, string goals, DateTime evaluationDate, string evaluatedBy)
        {
            ValidateDomain(employeeId, evaluationScore, feedback, goals, evaluationDate, evaluatedBy);
        }

        public EmployeeEvaluation(int id, int employeeId, int evaluationScore, string feedback, string goals, DateTime evaluationDate, string evaluatedBy)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(employeeId, evaluationScore, feedback, goals, evaluationDate, evaluatedBy);
        }

        public void Update(int evaluationScore, string feedback, string goals, string evaluatedBy)
        {
            ValidateDomain(this.EmployeeId, evaluationScore, feedback, goals, this.EvaluationDate, evaluatedBy);
        }

        private void ValidateDomain(int employeeId, int evaluationScore, string feedback, string goals, DateTime evaluationDate, string evaluatedBy)
        {
            DomainExceptionValidation.When(employeeId <= 0,
                "Invalid employee id. Employee id is required");

            DomainExceptionValidation.When(evaluationScore < 0 || evaluationScore > 100,
                "Invalid evaluation score. Score must be between 0 and 100");

            DomainExceptionValidation.When(string.IsNullOrEmpty(feedback),
                "Invalid feedback. Feedback is required");

            DomainExceptionValidation.When(string.IsNullOrEmpty(goals),
                "Invalid goals. Goals are required");

            DomainExceptionValidation.When(evaluationDate > DateTime.Now,
                "Invalid evaluation date. Evaluation date cannot be in the future");

            DomainExceptionValidation.When(string.IsNullOrEmpty(evaluatedBy),
                "Invalid evaluator. Evaluator is required");

            EmployeeId = employeeId;
            EvaluationScore = evaluationScore;
            Feedback = feedback;
            Goals = goals;
            EvaluationDate = evaluationDate;
            EvaluatedBy = evaluatedBy;
        }
    }
}