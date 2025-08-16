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

            //data sales
            var dataSales = await GetSales(filter);
            var salesList = dataSales.Items;

            if(salesList.Any()) {
                
                dashboard.Sales.QuantitySales = salesList.Count();
                dashboard.Sales.SalesSubtotal = salesList.Sum(s => s.SubTotal);
                dashboard.Sales.SalesIVA = salesList.Sum(s => s.Iva);
                dashboard.Sales.SalesTotal = salesList.Sum(s => s.Total);
                dashboard.Sales.SalesCambio = salesList.Average(s => s.Pay - s.Total);
                dashboard.Sales.AverageTicket = salesList.Average(s => s.Total);
                dashboard.Sales.AveragePayment = salesList.Average(s => s.Pay);

                //factura
                var invoices = salesList.Where(s => s.Invoice != 0).ToList();
                dashboard.Sales.InvoiceCount = salesList.Count(s => s.Invoice != 0);
                dashboard.Sales.InvoicedIVA = salesList.Where(s => s.Invoice != 0).Sum(s => s.Iva);
                dashboard.Sales.InvoicedTotal = invoices.Sum(s => s.Total);
                dashboard.Sales.InvoicedPercentage = dashboard.Sales.InvoiceCount > 0 ? (invoices.Count * 100.0 / salesList.Count()) : 0;
                dashboard.Sales.NotInvoicedPercentage = 100 - dashboard.Sales.InvoicedPercentage;


                //daily sales
                dashboard.DailySales = salesList
                    .GroupBy(s => s.CreateDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new DailySalesSummary {
                        Day = g.Key.Day,
                        SalesNumber = g.Count(),
                        Sales = g.ToList()
                    })
                    .ToList();

                //Sellers
                dashboard.Sellers = salesList
                    .GroupBy(s => s.Seller.Id)
                    .Select(g => new SellerSummary {
                        Id = g.Key,
                        Name = g.First().Seller.Name,
                        SalesNumber = g.Count(),
                        SalesTotal = g.Sum(s => s.Total),
                        AverageTicket = g.Average(s => s.Total)
                    })
                    .ToList();

                //Top 3
                dashboard.TopSellers = dashboard.Sellers
                    .OrderByDescending(v => v.SalesNumber)
                    .Take(3)
                    .ToList();
            }
            else {

                dashboard.Sales = new SaleSummary();
                dashboard.DailySales = new List<DailySalesSummary>();
                dashboard.Sellers = new List<SellerSummary>();
                dashboard.TopSellers = new List<SellerSummary>();
            }

            return dashboard;
        }

        public async Task<PagedResponse<SaleRES>> GetSales(SaleFilter filter) {

            filter.PageSize = 1000;

            //query
            var query = _unitOfWork.Sale
                .GetQuery()
                .Include(sale => sale.SellerNavigation)
                .Include(sale => sale.TbInvoices)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);
            if(filter.DateStart != null)
                query = query.Where(x => x.CreateDate.Date >= filter.DateStart.Value.Date);
            if(filter.DateFinish != null)
                query = query.Where(x => x.CreateDate.Date <= filter.DateFinish.Value.Date);

            int totalItems = await query.CountAsync();

            //select
            var sales = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(sale => new SaleRES {
                    Id = sale.Id,
                    Seller = new GenericCatalog {
                        Id = sale.SellerNavigation.Id,
                        Name = $"{sale.SellerNavigation.FirstName} {sale.SellerNavigation.LastName}"
                    },
                    SubTotal = sale.SubTotal,
                    Iva = sale.IVA,
                    Total = sale.Total,
                    Pay = sale.Pay,
                    CreateDate = sale.CreateDate,
                    SaleDetails = new List<SaleDetail>(),
                    Invoice = sale.TbInvoices.Count() == 0 ? 0 : sale.TbInvoices.FirstOrDefault().Id
                }).ToListAsync();

            //response
            return new PagedResponse<SaleRES> {
                Items = sales,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}
