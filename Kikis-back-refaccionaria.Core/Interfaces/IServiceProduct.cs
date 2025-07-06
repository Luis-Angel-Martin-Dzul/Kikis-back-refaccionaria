using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceProduct {

        /*
         *  GET
         */
        Task<PagedResponse<ProductRES>> GetProducts(ProductFilter filter, string schema);


        /*
         *  DELETE
         */
        Task<bool> DeleteProduct(int id);


        /*
         *  POST
         */
        Task<ProductRES> PostProduct(ProductREQ request, string schema);


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
