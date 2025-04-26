using HelpApp.Domain.Entities;
using HelpApp.Domain.Interfaces;
using HelpApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpApp.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _productContext;

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
            return await _productContext.Products.Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productContext.Products.Include(p => p.Category).ToListAsync();
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
    }
}

