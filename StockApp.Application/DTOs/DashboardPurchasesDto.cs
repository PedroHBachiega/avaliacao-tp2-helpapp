using System.Collections.Generic;


namespace StockApp.Application.DTOs
{
   public class DashboardPurchasesDto
    {
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public List<SupplierPurchasesDto> TopSuppliers { get; set; }
    }
}
