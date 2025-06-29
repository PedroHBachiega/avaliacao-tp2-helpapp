using StockApp.Domain.Validation;
using System;

namespace StockApp.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public bool IsApproved { get; set; }

        public Review()
        {
        }

        public Review(int productId, string userId, int rating, string comment)
        {
            ValidateDomain(productId, userId, rating, comment);
            Date = DateTime.UtcNow;
            IsApproved = false;
        }

        public Review(int id, int productId, string userId, int rating, string comment, DateTime date, bool isApproved)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            Id = id;
            ValidateDomain(productId, userId, rating, comment);
            Date = date;
            IsApproved = isApproved;
        }

        public void Update(int rating, string comment)
        {
            DomainExceptionValidation.When(rating < 1 || rating > 5, "Rating must be between 1 and 5.");
            DomainExceptionValidation.When(string.IsNullOrEmpty(comment), "Comment is required.");
            DomainExceptionValidation.When(comment.Length > 1000, "Comment cannot exceed 1000 characters.");
            
            Rating = rating;
            Comment = comment;
        }

        public void Approve()
        {
            IsApproved = true;
        }

        public void Reject()
        {
            IsApproved = false;
        }

        private void ValidateDomain(int productId, string userId, int rating, string comment)
        {
            DomainExceptionValidation.When(productId <= 0, "Invalid ProductId value.");
            DomainExceptionValidation.When(string.IsNullOrEmpty(userId), "UserId is required.");
            DomainExceptionValidation.When(rating < 1 || rating > 5, "Rating must be between 1 and 5.");
            DomainExceptionValidation.When(string.IsNullOrEmpty(comment), "Comment is required.");
            DomainExceptionValidation.When(comment.Length > 1000, "Comment cannot exceed 1000 characters.");

            ProductId = productId;
            UserId = userId;
            Rating = rating;
            Comment = comment;
        }

        public Product Product { get; set; }
    }
}