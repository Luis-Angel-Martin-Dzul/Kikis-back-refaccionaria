using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceSupplier {

        /*
         *  GET
         */
        Task<IEnumerable<SupplierRES>> GetSupplier();


        /*
         *  DELETE
         */
        Task<bool> DeleteSupplier(int id);


        /*
         *  POST
         */
        Task<SupplierRES> PostSupplier(SupplierRES request);


        /*
         *  PUT
         */
        Task<SupplierRES> PutSupplier(SupplierRES request);
    }
}
