using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceSale {

        /*
         *  GET
         */
        Task<IEnumerable<SaleRES>> GetSales(SaleFilter filter);
        Task<IEnumerable<InvoiceRES>> GetInvoices(InvoiceFilter filter);


        /*
         *  POST
         */
        Task<bool> PostSales(SaleREQ request);
        Task<int> PostInvoice(InvoiceREQ request);
        Task<bool> PostTryInvoice(InvoiceTryREQ request);
        Task<bool> PostQuote(SaleREQ request);


        /*
         *  PUT
         */
    }
}
