using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class AdvancedSearchDTO : PaginationParameters
    {
        /// <summary>
        /// Busca por texto livre (nome, descrição)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filtro por nome específico
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Filtro por descrição específica
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Filtro por categoria
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Filtro por múltiplas categorias
        /// </summary>
        public List<int>? CategoryIds { get; set; }

        /// <summary>
        /// Preço mínimo
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Preço máximo
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Estoque mínimo
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be greater than or equal to 0")]
        public int? MinStock { get; set; }

        /// <summary>
        /// Estoque máximo
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Maximum stock must be greater than or equal to 0")]
        public int? MaxStock { get; set; }

        /// <summary>
        /// Filtro para produtos com estoque baixo
        /// </summary>
        public bool? IsLowStock { get; set; }

        /// <summary>
        /// Filtro para produtos em promoção
        /// </summary>
        public bool? HasPromotion { get; set; }

        /// <summary>
        /// Filtro por faixa de desconto mínimo
        /// </summary>
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public decimal? MinDiscountPercentage { get; set; }

        /// <summary>
        /// Campo para ordenação
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Direção da ordenação (asc/desc)
        /// </summary>
        public string? SortDirection { get; set; } = "asc";

        /// <summary>
        /// Ordenação secundária
        /// </summary>
        public string? SecondarySortBy { get; set; }

        /// <summary>
        /// Direção da ordenação secundária
        /// </summary>
        public string? SecondarySortDirection { get; set; } = "asc";

        /// <summary>
        /// Incluir produtos sem categoria
        /// </summary>
        public bool IncludeWithoutCategory { get; set; } = true;

        /// <summary>
        /// Busca exata (não usar LIKE)
        /// </summary>
        public bool ExactMatch { get; set; } = false;

        /// <summary>
        /// Busca case-sensitive
        /// </summary>
        public bool CaseSensitive { get; set; } = false;

        public bool IsValidPriceRange()
        {
            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                return MinPrice.Value <= MaxPrice.Value;
            }
            return true;
        }

        public bool IsValidStockRange()
        {
            if (MinStock.HasValue && MaxStock.HasValue)
            {
                return MinStock.Value <= MaxStock.Value;
            }
            return true;
        }

        public bool IsValid()
        {
            return IsValidPriceRange() && IsValidStockRange();
        }

        public List<string> GetValidationErrors()
        {
            var errors = new List<string>();

            if (!IsValidPriceRange())
            {
                errors.Add("Minimum price must be less than or equal to maximum price");
            }

            if (!IsValidStockRange())
            {
                errors.Add("Minimum stock must be less than or equal to maximum stock");
            }

            return errors;
        }
    }
}