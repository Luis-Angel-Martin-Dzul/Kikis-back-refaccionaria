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



        public async Task<DashboardRES> getSales(SaleFilter filter) {
            var dashboard = new DashboardRES {
                Sale = new SaleD2(),
                Vendedores = new List<VendedorRES>(),
                TopVendedores = new List<VendedorRES>()
            };

            // Limitar para obtener todas las ventas
            filter.PageSize = 1000;
            var pagedSales = await _serviceSale.GetSales(filter);
            var salesList = pagedSales.Items;

            if(salesList.Any()) {
                // -------- Métricas generales --------
                dashboard.Sale.Total = salesList.Sum(s => s.Total);
                dashboard.Sale.Subtotal = salesList.Sum(s => s.SubTotal);
                dashboard.Sale.Iva = salesList.Sum(s => s.Iva);
                dashboard.Sale.CantidadVentas = salesList.Count();
                dashboard.Sale.TicketPromedio = salesList.Average(s => s.Total);
                dashboard.Sale.PagoPromedio = salesList.Average(s => s.Pay);
                dashboard.Sale.Cambio = salesList.Average(s => s.Pay - s.Total);

                // -------- Ventas por día --------
                dashboard.VentasPorDia = salesList
                    .GroupBy(s => s.CreateDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new VentasDiaRES {
                        Dia = g.Key.Day,
                        NumeroVentas = g.Count(),
                        products = g.ToList()
                    })
                    .ToList();

                // -------- Lista de vendedores --------
                dashboard.Vendedores = salesList
                    .GroupBy(s => s.Seller.Id) // Agrupar por vendedor
                    .Select(g => new VendedorRES {
                        SellerId = g.Key,
                        SellerName = g.First().Seller.Name,
                        NumeroVentas = g.Count(),
                        TotalVentas = g.Sum(s => s.Total),
                        TicketPromedio = g.Average(s => s.Total)
                    })
                    .ToList();

                // -------- Top 3 vendedores por número de ventas --------
                dashboard.TopVendedores = dashboard.Vendedores
                    .OrderByDescending(v => v.NumeroVentas)
                    .Take(3)
                    .ToList();
            }
            else {
                dashboard.VentasPorDia = new List<VentasDiaRES>();
                dashboard.Vendedores = new List<VendedorRES>();
                dashboard.TopVendedores = new List<VendedorRES>();
            }

            return dashboard;
        }



    }
}
