using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessReportsManager.Application.DTOs
{
    public class OrderFinancialSummaryDto
    {
        public Guid OrderId { get; set; }
        public decimal SellPriceInGel { get; set; }
        public decimal TotalExpenseInGel { get; set; }
        public decimal TotalPaidInGel { get; set; }
        public decimal CustomerRemainingInGel { get; set; }
        public decimal ProfitInGel { get; set; }
        public decimal CashFlowInGel { get; set; }
    }
}
