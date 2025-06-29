using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class ProductComparisonDTO
    {
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public ProductComparisonSummaryDTO Summary { get; set; } = new ProductComparisonSummaryDTO();
        public DateTime ComparedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProductComparisonSummaryDTO
    {
        public decimal HighestPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal PriceDifference { get; set; }
        public int HighestStock { get; set; }
        public int LowestStock { get; set; }
        public double AverageStock { get; set; }
        public int StockDifference { get; set; }
        public ProductDTO MostExpensive { get; set; }
        public ProductDTO Cheapest { get; set; }
        public ProductDTO HighestStockProduct { get; set; }
        public ProductDTO LowestStockProduct { get; set; }
        public List<string> CommonCategories { get; set; } = new List<string>();
        public int TotalProductsCompared { get; set; }
    }

    public class ProductComparisonRequestDTO
    {
        [Required(ErrorMessage = "Lista de IDs de produtos é obrigatória")]
        [MinLength(2, ErrorMessage = "É necessário pelo menos 2 produtos para comparação")]
        [MaxLength(10, ErrorMessage = "Máximo de 10 produtos podem ser comparados por vez")]
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}