using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IService {

        #region User

        /*
         *  GET
         */
        Task<IEnumerable<RolRES>> GetRols(RolFilter filter);
        Task<IEnumerable<UserRES>> GetUsers(UserFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteUser(int id);


        /*
         *  POST
         */
        Task<UserRES> PostUser(UserREQ request);
        Task<UserAuthRES> Login(AuthREQ request);
        Task<bool> PostRols(RolRES request);


        /*
         *  PUT
         */
        Task<bool> PutRols(RolRES request);
        Task<UserRES> PutUser(UserREQ request);


        #endregion

        #region Product

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

        #endregion

        #region Catalogs
        /*
         *  GET
         */
        Task<IEnumerable<GenericCatalog>> GetProductCategory();
        Task<IEnumerable<GenericCatalog>> GetProductBrand();
        Task<IEnumerable<GenericCatalog>> GetProductHallway();
        Task<IEnumerable<GenericCatalog>> GetProductLevel();
        Task<IEnumerable<GenericCatalog>> GetProductShelf();
        Task<IEnumerable<GenericCatalog>> GetProductKit();

        /*
         *  POST
         */

        /*
         *  PUT
         */
        #endregion

        #region Sale
        /*
         *  GET
         */
        Task<IEnumerable<SaleRES>> GetSales(SaleFilter filter);


        /*
         *  POST
         */
        Task<bool> PostSales(SaleREQ request);


        /*
         *  PUT
         */
        #endregion

        #region Supplier
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

        #endregion

        #region Generic
        /*
         *  GET
         */

        /*
         *  POST
         */

        /*
         *  PUT
         */
        #endregion

    }
}
