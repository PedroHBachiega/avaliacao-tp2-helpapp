using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StockApp.Infra.Data.Context;
using StockApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace StockApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-purchases")]
        public async Task<IActionResult> GetDashboardPurchasesData()
        {
            var dashboardData = new DashboardPurchasesDto
            {
                TotalPurchases = await _context.Purchases.CountAsync(),
                TotalSpent = await _context.Purchases.SumAsync(p => p.Quantity * p.Price),
                TopSuppliers = await _context.Suppliers
                    .OrderByDescending(s => s.Purchases.Sum(p => p.Quantity))
                    .Take(5)
                    .Select(s => new SupplierPurchasesDto
                    {
                        SupplierName = s.Name,
                        TotalPurchased = s.Purchases.Sum(p => p.Quantity)
                    })
                    .ToListAsync()
            };

            return Ok(dashboardData);
        }
    }
}
