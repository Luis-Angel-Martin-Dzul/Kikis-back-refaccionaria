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

        #region Tool

        /*
         *  GET
         */
        Task<IEnumerable<ToolRES>> GetTools(ToolFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteTool(int id);


        /*
         *  POST
         */
        Task<ToolRES> PostTool(ToolREQ request);


        /*
         *  PUT
         */
        Task<bool> PutToolPromotion(ToolPromotionREQ request);
        Task<bool> PutToolStock(ToolStockREQ request);

        #endregion

        #region Catalogs
        /*
         *  GET
         */
        Task<IEnumerable<GenericCatalog>> GetToolCategory();
        Task<IEnumerable<GenericCatalog>> GetToolBrand();

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
