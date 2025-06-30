using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace StockApp.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        ApplicationDbContext _productContext;
        public ProductRepository(ApplicationDbContext context)
        {
            _productContext = context;
        }

        public async Task<Product> Create(Product product)
        {
            _productContext.Add(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetById(int? id)
        {
            return await _productContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productContext.Products.ToListAsync();
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> GetProductsPaged(int pageNumber, int pageSize)
        {
            var totalCount = await _productContext.Products.CountAsync();
            
            var products = await _productContext.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<Product> Remove(Product product)
        {
            _productContext.Remove(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Update(Product product)
        {
            _productContext.Update(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
        {
            return await _productContext.Products
                .Where(p => p.Stock < threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productContext.Products.ToListAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _productContext.Products.Update(product);
            await _productContext.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> GetProductsWithFiltersAsync(
            int pageNumber, 
            int pageSize,
            string? name = null,
            string? description = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isLowStock = null,
            string? sortBy = null,
            string? sortDirection = "asc")
        {
            var query = _productContext.Products.Include(p => p.Category).AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query.Where(p => p.Description.Contains(description));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (minStock.HasValue)
            {
                query = query.Where(p => p.Stock >= minStock.Value);
            }

            if (maxStock.HasValue)
            {
                query = query.Where(p => p.Stock <= maxStock.Value);
            }

            if (isLowStock.HasValue && isLowStock.Value)
            {
                query = query.Where(p => p.Stock < p.MinimumStockLevel);
            }

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(sortBy))
            {
                var isDescending = sortDirection?.ToLower() == "desc";
                
                query = sortBy.ToLower() switch
                {
                    "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "stock" => isDescending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
                    "category" => isDescending ? query.OrderByDescending(p => p.Category.Name) : query.OrderBy(p => p.Category.Name),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // Contar total de registros
            var totalCount = await query.CountAsync();

            // Aplicar paginação
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> AdvancedSearchAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? name = null,
            string? description = null,
            int? categoryId = null,
            List<int>? categoryIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isLowStock = null,
            bool? hasPromotion = null,
            decimal? minDiscountPercentage = null,
            string? sortBy = null,
            string? sortDirection = "asc",
            string? secondarySortBy = null,
            string? secondarySortDirection = "asc",
            bool includeWithoutCategory = true,
            bool exactMatch = false,
            bool caseSensitive = false)
        {
            var query = _productContext.Products.Include(p => p.Category).AsQueryable();

            // Busca por termo geral (nome ou descrição)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (exactMatch)
                {
                    if (caseSensitive)
                    {
                        query = query.Where(p => p.Name == searchTerm || p.Description == searchTerm);
                    }
                    else
                    {
                        query = query.Where(p => p.Name.ToLower() == searchTerm.ToLower() || 
                                                p.Description.ToLower() == searchTerm.ToLower());
                    }
                }
                else
                {
                    if (caseSensitive)
                    {
                        query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
                    }
                    else
                    {
                        var lowerSearchTerm = searchTerm.ToLower();
                        query = query.Where(p => p.Name.ToLower().Contains(lowerSearchTerm) || 
                                                p.Description.ToLower().Contains(lowerSearchTerm));
                    }
                }
            }

            // Filtro por nome específico
            if (!string.IsNullOrEmpty(name))
            {
                if (exactMatch)
                {
                    query = caseSensitive ? 
                        query.Where(p => p.Name == name) : 
                        query.Where(p => p.Name.ToLower() == name.ToLower());
                }
                else
                {
                    query = caseSensitive ? 
                        query.Where(p => p.Name.Contains(name)) : 
                        query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
                }
            }

            // Filtro por descrição específica
            if (!string.IsNullOrEmpty(description))
            {
                if (exactMatch)
                {
                    query = caseSensitive ? 
                        query.Where(p => p.Description == description) : 
                        query.Where(p => p.Description.ToLower() == description.ToLower());
                }
                else
                {
                    query = caseSensitive ? 
                        query.Where(p => p.Description.Contains(description)) : 
                        query.Where(p => p.Description.ToLower().Contains(description.ToLower()));
                }
            }

            // Filtro por categoria única
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Filtro por múltiplas categorias
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(p => categoryIds.Contains(p.CategoryId));
            }

            // Filtro para incluir produtos sem categoria
            if (!includeWithoutCategory)
            {
                query = query.Where(p => p.CategoryId > 0);
            }

            // Filtros de preço
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Filtros de estoque
            if (minStock.HasValue)
            {
                query = query.Where(p => p.Stock >= minStock.Value);
            }

            if (maxStock.HasValue)
            {
                query = query.Where(p => p.Stock <= maxStock.Value);
            }

            // Filtro para estoque baixo
            if (isLowStock.HasValue && isLowStock.Value)
            {
                query = query.Where(p => p.Stock < p.MinimumStockLevel);
            }

            // Filtro para produtos em promoção
            if (hasPromotion.HasValue)
            {
                if (hasPromotion.Value)
                {
                    query = query.Where(p => p.DiscountPercentage.HasValue && p.DiscountPercentage > 0);
                }
                else
                {
                    query = query.Where(p => !p.DiscountPercentage.HasValue || p.DiscountPercentage == 0);
                }
            }

            // Filtro por desconto mínimo
            if (minDiscountPercentage.HasValue)
            {
                query = query.Where(p => p.DiscountPercentage.HasValue && 
                                        p.DiscountPercentage >= minDiscountPercentage.Value);
            }

            // Aplicar ordenação primária
            if (!string.IsNullOrEmpty(sortBy))
            {
                var isDescending = sortDirection?.ToLower() == "desc";
                
                query = sortBy.ToLower() switch
                {
                    "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "stock" => isDescending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
                    "category" => isDescending ? query.OrderByDescending(p => p.Category.Name) : query.OrderBy(p => p.Category.Name),
                    "discount" => isDescending ? query.OrderByDescending(p => p.DiscountPercentage ?? 0) : query.OrderBy(p => p.DiscountPercentage ?? 0),
                    "id" => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                    _ => query.OrderBy(p => p.Id)
                };

                // Aplicar ordenação secundária
                if (!string.IsNullOrEmpty(secondarySortBy) && secondarySortBy != sortBy)
                {
                    var isSecondaryDescending = secondarySortDirection?.ToLower() == "desc";
                    
                    var orderedQuery = (IOrderedQueryable<Product>)query;
                    
                    query = secondarySortBy.ToLower() switch
                    {
                        "name" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.Name) : orderedQuery.ThenBy(p => p.Name),
                        "price" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.Price) : orderedQuery.ThenBy(p => p.Price),
                        "stock" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.Stock) : orderedQuery.ThenBy(p => p.Stock),
                        "category" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.Category.Name) : orderedQuery.ThenBy(p => p.Category.Name),
                        "discount" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.DiscountPercentage ?? 0) : orderedQuery.ThenBy(p => p.DiscountPercentage ?? 0),
                        "id" => isSecondaryDescending ? orderedQuery.ThenByDescending(p => p.Id) : orderedQuery.ThenBy(p => p.Id),
                        _ => orderedQuery.ThenBy(p => p.Id)
                    };
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // Contar total de registros
            var totalCount = await query.CountAsync();

            // Aplicar paginação
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}
