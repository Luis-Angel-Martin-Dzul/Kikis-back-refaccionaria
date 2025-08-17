using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceDashboard : IServiceDashboard {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceSale _serviceSale;
        public ServiceDashboard(IUnitOfWork unitOfWork, IServiceSale serviceSale) {
            _unitOfWork = unitOfWork;
            _serviceSale = serviceSale;
        }

        public async Task<GeneralSummary> getSales(SaleFilter filter) {
            var dashboard = new GeneralSummary {
                Sales = new SaleSummary(),
                DailySales = new List<DailySalesSummary>(),
                Sellers = new List<SellerSummary>(),
                TopSellers = new List<SellerSummary>()
            };

            var query = _unitOfWork.Sale
                .GetQuery()
                .Include(s => s.SellerNavigation)
                .Include(s => s.TbInvoices)
                .AsNoTracking();

            // filtros
            if(filter.DateStart != null)
                query = query.Where(x => x.CreateDate.Date >= filter.DateStart.Value.Date);
            if(filter.DateFinish != null)
                query = query.Where(x => x.CreateDate.Date <= filter.DateFinish.Value.Date);

            // 1️⃣ Totales de ventas
            var salesTotals = await query
                .GroupBy(x => 1) // un solo grupo
                .Select(g => new {
                    QuantitySales = g.Count(),
                    SalesSubtotal = g.Sum(s => s.SubTotal),
                    SalesIVA = g.Sum(s => s.IVA),
                    SalesTotal = g.Sum(s => s.Total),
                    SalesCambio = g.Average(s => s.Pay - s.Total),
                    AverageTicket = g.Average(s => s.Total),
                    AveragePayment = g.Average(s => s.Pay),
                    InvoiceCount = g.Count(s => s.TbInvoices.Any()),
                    InvoicedIVA = g.Where(s => s.TbInvoices.Any()).Sum(s => s.IVA),
                    InvoicedTotal = g.Where(s => s.TbInvoices.Any()).Sum(s => s.Total)
                })
                .FirstOrDefaultAsync();

            if(salesTotals != null) {
                dashboard.Sales.QuantitySales = salesTotals.QuantitySales;
                dashboard.Sales.SalesSubtotal = salesTotals.SalesSubtotal;
                dashboard.Sales.SalesIVA = salesTotals.SalesIVA;
                dashboard.Sales.SalesTotal = salesTotals.SalesTotal;
                dashboard.Sales.SalesCambio = salesTotals.SalesCambio;
                dashboard.Sales.AverageTicket = salesTotals.AverageTicket;
                dashboard.Sales.AveragePayment = salesTotals.AveragePayment;
                dashboard.Sales.InvoiceCount = salesTotals.InvoiceCount;
                dashboard.Sales.InvoicedIVA = salesTotals.InvoicedIVA;
                dashboard.Sales.InvoicedTotal = salesTotals.InvoicedTotal;
                dashboard.Sales.InvoicedPercentage = salesTotals.QuantitySales > 0 ? (salesTotals.InvoiceCount * 100.0 / salesTotals.QuantitySales) : 0;
                dashboard.Sales.NotInvoicedPercentage = 100 - dashboard.Sales.InvoicedPercentage;
            }

            // 2️⃣ Ventas diarias
            dashboard.DailySales = await query
                .GroupBy(s => s.CreateDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailySalesSummary {
                    Day = g.Key.Day,
                    SalesNumber = g.Count()
                })
                .ToListAsync();

            // 3️⃣ Vendedores y top 3
            var sellers = await query
                .GroupBy(s => s.SellerNavigation.Id)
                .Select(g => new SellerSummary {
                    Id = g.Key,
                    Name = g.First().SellerNavigation.FirstName + " " + g.First().SellerNavigation.LastName,
                    SalesNumber = g.Count(),
                    SalesTotal = g.Sum(s => s.Total),
                    AverageTicket = g.Average(s => s.Total)
                })
                .ToListAsync();

            dashboard.Sellers = sellers;
            dashboard.TopSellers = sellers.OrderByDescending(s => s.SalesNumber).Take(3).ToList();

            return dashboard;
        }

    }
}
