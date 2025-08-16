using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceDashboard {

        Task<DashboardRES> getSales(SaleFilter filter);

    }
}
