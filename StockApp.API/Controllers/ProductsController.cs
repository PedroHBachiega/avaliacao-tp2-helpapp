using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using System.Security.Claims;
using System;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace StockApp.API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de produtos
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly IProductRepository _productRepository;
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;
    public ProductsController(IProductService productService, IProductRepository productRepository, ICacheService cache, IReviewService reviewService)
    {
        _productService = productService;
        _productRepository = productRepository;
        _cache = cache;
        _reviewService = reviewService;
    }

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    /// <returns>Lista de produtos</returns>
    /// <response code="200">Retorna a lista de produtos</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        const string cacheKey = "products_all";

        var cachedProducts = await _cache.GetAsync<List<ProductDTO>>(cacheKey);
        if (cachedProducts != null)
        {
            return Ok(cachedProducts);
        }

        var products = await _productService.GetProducts();

        await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(10));

        return Ok(products);
    }

    /// <summary>
    /// Obtém produtos com paginação
    /// </summary>
    /// <param name="paginationParameters">Parâmetros de paginação</param>
    /// <returns>Lista paginada de produtos</returns>
    /// <response code="200">Retorna a lista paginada de produtos</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<ProductDTO>>> GetAllPaged([FromQuery] PaginationParameters paginationParameters)
    {
        var pagedProducts = await _productService.GetProductsPaged(paginationParameters);
        return Ok(pagedProducts);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductDTO>>> SearchProducts([FromQuery] ProductSearchDTO searchParameters)
    {
        if (!searchParameters.IsValid())
        {
            var errors = searchParameters.GetValidationErrors();
            return BadRequest(new { errors });
        }

        var filteredProducts = await _productService.GetProductsWithFiltersAsync(searchParameters);
        return Ok(filteredProducts);
    }

    /// <summary>
    /// Busca avançada de produtos com múltiplos filtros e opções de ordenação
    /// </summary>
    /// <param name="searchParameters">Parâmetros de busca avançada</param>
    /// <returns>Lista paginada de produtos filtrados</returns>
    /// <response code="200">Retorna a lista paginada de produtos filtrados</response>
    /// <response code="400">Parâmetros de busca inválidos</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("advanced-search")]
    public async Task<ActionResult<PagedResult<ProductDTO>>> AdvancedSearch([FromQuery] AdvancedSearchDTO searchParameters)
    {
        try
        {
            if (!searchParameters.IsValid())
            {
                var errors = searchParameters.GetValidationErrors();
                return BadRequest(new { 
                    message = "Invalid search parameters",
                    errors = errors 
                });
            }

            var searchResults = await _productService.AdvancedSearchAsync(searchParameters);
            
            return Ok(new {
                data = searchResults,
                searchCriteria = new {
                    searchTerm = searchParameters.SearchTerm,
                    categoryFilter = searchParameters.CategoryId.HasValue ? new List<int> { searchParameters.CategoryId.Value } : searchParameters.CategoryIds,
                    priceRange = new { min = searchParameters.MinPrice, max = searchParameters.MaxPrice },
                    stockRange = new { min = searchParameters.MinStock, max = searchParameters.MaxStock },
                    hasPromotion = searchParameters.HasPromotion,
                    sortBy = searchParameters.SortBy,
                    sortDirection = searchParameters.SortDirection
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Busca sugestões de produtos para autocomplete
    /// </summary>
    /// <param name="term">Termo de busca</param>
    /// <param name="limit">Limite de sugestões (padrão: 10)</param>
    /// <returns>Lista de sugestões</returns>
    /// <response code="200">Retorna lista de sugestões</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("suggestions")]
    public async Task<ActionResult<IEnumerable<object>>> GetSuggestions([FromQuery] string term, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        {
            return Ok(new List<object>());
        }

        try
        {
            var searchParams = new AdvancedSearchDTO
            {
                SearchTerm = term,
                PageNumber = 1,
                PageSize = limit,
                SortBy = "name",
                SortDirection = "asc"
            };

            var results = await _productService.AdvancedSearchAsync(searchParams);
            
            var suggestions = results.Items.Select(p => new {
                id = p.Id,
                name = p.Name,
                price = p.Price,
                category = p.Category?.Name,
                stock = p.Stock
            });

            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting suggestions", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtém estatísticas de busca para dashboard
    /// </summary>
    /// <returns>Estatísticas dos produtos</returns>
    /// <response code="200">Retorna estatísticas</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("search-stats")]
    public async Task<ActionResult<object>> GetSearchStats()
    {
        try
        {
            var allProducts = await _productService.GetProducts();
            var productList = allProducts.ToList();

            var stats = new {
                totalProducts = productList.Count,
                averagePrice = productList.Any() ? productList.Average(p => p.Price) : 0,
                totalStock = productList.Sum(p => p.Stock),
                lowStockCount = productList.Count(p => p.Stock < 10),
                promotionCount = productList.Count(p => p.DiscountPercentage.HasValue && p.DiscountPercentage > 0),
                priceRange = new {
                    min = productList.Any() ? productList.Min(p => p.Price) : 0,
                    max = productList.Any() ? productList.Max(p => p.Price) : 0
                },
                stockRange = new {
                    min = productList.Any() ? productList.Min(p => p.Stock) : 0,
                    max = productList.Any() ? productList.Max(p => p.Stock) : 0
                },
                categoriesCount = productList.GroupBy(p => p.Category?.Name).Count()
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting search statistics", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um produto específico pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Produto encontrado</returns>
    /// <response code="200">Retorna o produto encontrado</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetById(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Obtém produtos com estoque baixo
    /// </summary>
    /// <param name="threshold">Limite mínimo de estoque</param>
    /// <returns>Lista de produtos com estoque baixo</returns>
    /// <response code="200">Retorna produtos com estoque baixo</response>
    /// <response code="401">Não autorizado</response>
    [HttpGet("low stock")]
    public async Task<ActionResult<IEnumerable<Product>>> GetLowStock([FromQuery] int threshold)
    {
        var products = await _productRepository.GetLowStockAsync(threshold);
        return Ok(products);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="productDTO">Dados do produto a ser criado</param>
    /// <returns>Produto criado</returns>
    /// <response code="201">Produto criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost]
    public async Task<ActionResult<ProductDTO>> Create([FromBody] ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            return BadRequest("Invalid Data");
        }
        await _productService.Add(productDTO);
        return CreatedAtAction(nameof(GetById), new { id = productDTO.Id }, productDTO);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto a ser atualizado</param>
    /// <param name="productDto">Dados atualizados do produto</param>
    /// <returns>Produto atualizado</returns>
    /// <response code="200">Produto atualizado com sucesso</response>
    /// <response code="400">Dados inválidos ou ID não corresponde</response>
    /// <response code="401">Não autorizado</response>


    /// <summary>
    /// Remove um produto pelo ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Produto removido</returns>
    /// <response code="200">Produto removido com sucesso</response>
    /// <response code="404">Produto não encontrado</response>
    /// <response code="401">Não autorizado</response>
    [HttpDelete("{id:int}", Name = "DeleteProduct")]
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound("Product not found");
        }
        await _productService.Remove(id);
        return Ok(product);
    }

    /// <summary>
    /// Atualiza vários produtos em massa
    /// </summary>
    /// <param name="bulkUpdateDto">DTO contendo a lista de produtos a serem atualizados</param>
    /// <returns>Status da operação</returns>
    /// <response code="200">Atualização realizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPut("bulk-update")]
    public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateProductDTO bulkUpdateDto)
    {
        if (bulkUpdateDto?.Products == null || !bulkUpdateDto.Products.Any())
            return BadRequest("Lista de produtos não pode ser vazia.");

        foreach (var product in bulkUpdateDto.Products)
        {
            await _productService.Update(product);
        }
        // Opcional: Limpar cache relacionado
        await _cache.RemoveAsync("products_all");
        return Ok(new { message = "Produtos atualizados com sucesso." });
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportFromCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Arquivo inválido.");

        using (var stream = new StreamReader(file.OpenReadStream()))
        {
            while (!stream.EndOfStream)
            {
                var line = await stream.ReadLineAsync();
                var values = line.Split(',');

                var productDTO = new ProductDTO
                {
                    Name = values[0],
                    Description = values[1],
                    Price = decimal.Parse(values[2]),
                    Stock = int.Parse(values[3])
                };

                await _productService.Add(productDTO);
            }
        }

        await _cache.RemoveAsync("products_all");

        return Ok(new { message = "Importação concluída com sucesso." });
    }

    [HttpPost("compare")]
    public async Task<ActionResult<ProductComparisonDTO>> CompareProducts([FromBody] ProductComparisonRequestDTO request)
    {
        if (request?.ProductIds == null || request.ProductIds.Count < 2)
        {
            return BadRequest("É necessário pelo menos 2 produtos para comparação.");
        }

        if (request.ProductIds.Count > 10)
        {
            return BadRequest("Máximo de 10 produtos podem ser comparados por vez.");
        }

        try
        {
            var comparison = await _productService.CompareProductsAsync(request.ProductIds);
            return Ok(comparison);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor ao comparar produtos.");
        }
    }

    [HttpGet("compare/category/{categoryId}")]
    public async Task<ActionResult<ProductComparisonDTO>> CompareProductsByCategory(int categoryId, [FromQuery] int limit = 5)
    {
        if (limit < 2) limit = 2;
        if (limit > 10) limit = 10;

        try
        {
            var searchParams = new ProductSearchDTO
            {
                PageNumber = 1,
                PageSize = limit,
                Filters = new ProductFilterDTO
                {
                    CategoryId = categoryId
                }
            };

            var categoryProducts = await _productService.GetProductsWithFiltersAsync(searchParams);
            
            if (categoryProducts.Data.Count() < 2)
            {
                return BadRequest("Categoria não possui produtos suficientes para comparação.");
            }

            var productIds = categoryProducts.Data.Select(p => p.Id).ToList();
            var comparison = await _productService.CompareProductsAsync(productIds);
            
            return Ok(comparison);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor ao comparar produtos da categoria.");
        }
    }

    [HttpPost("{productId}/review")]
    [Authorize]
    public async Task<IActionResult> AddReview(int productId, [FromBody] CreateReviewDTO review)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var createdReview = await _reviewService.CreateReviewAsync(productId, userId, review);
            return Ok(createdReview);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor ao adicionar avaliação.");
        }
    }

    [HttpGet("{productId}/reviews")]
    public async Task<IActionResult> GetProductReviews(int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var reviews = await _reviewService.GetProductReviewsPagedAsync(productId, pageNumber, pageSize);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor ao buscar avaliações.");
        }
    }

    [HttpGet("{productId}/reviews/summary")]
    public async Task<IActionResult> GetProductReviewSummary(int productId)
    {
        try
        {
            var summary = await _reviewService.GetReviewSummaryByProductIdAsync(productId);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor ao buscar resumo de avaliações.");
        }
    }

   
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDTO productDto)
    {
        try
        {
            if (productDto == null)
                return BadRequest("Dados de produto inválidos");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (productDto.Id != 0 && productDto.Id != id)
                return BadRequest("Incompatibilidade entre o ID da rota e o ID do produto");
        
            var existingProduct = await _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound($"Produto com ID {id} não encontrado");
       
            // Atualiza o produto com novos valores usando o método existente
            await _productService.Update(productDto);
        
            // Obtém o produto atualizado
            var updatedProduct = await _productService.GetProductById(id);
        
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            // Registre a exceção aqui (adicione um logger real)
            // _logger.LogError(ex, "Erro ao atualizar o produto {ProductId}", id);
            return StatusCode(500, "Ocorreu um erro ao atualizar o produto");
        }
    }
}