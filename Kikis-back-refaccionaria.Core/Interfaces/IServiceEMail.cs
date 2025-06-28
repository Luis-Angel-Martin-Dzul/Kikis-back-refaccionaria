using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceEMail {

        bool SendUserPasswordEmail(string to, string password);
        bool SendCFDI(TbInvoice invoice, SaleRES product);
        bool SendQuote(SaleREQ quote);
    }
}
