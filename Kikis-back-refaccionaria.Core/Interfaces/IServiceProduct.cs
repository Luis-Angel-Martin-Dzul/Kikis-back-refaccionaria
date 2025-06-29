using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceProduct {

        /*
         *  GET
         */
        Task<IEnumerable<ProductRES>> GetProducts(ProductFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteProduct(int id);


        /*
         *  POST
         */
        Task<ProductRES> PostProduct(ProductREQ request);


        /*
         *  PUT
         */
        Task<bool> PutProductPromotion(ProductPromotionREQ request);
        Task<bool> PutProductStock(ProductStockREQ request);
        Task<bool> PutProductWarehouse(ProductWarehouseREQ request);
        Task<bool> PutProductSupplier(ProductRES request);
        Task<bool> PutProductKit(ProductRES request);
    }
}
