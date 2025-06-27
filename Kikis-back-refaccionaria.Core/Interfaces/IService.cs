using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IService {

        #region Client

        /*
         *  GET
         */
        Task<IEnumerable<ClientRES>> GetClients(ClientFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteClient(int id);



        /*
         *  POST
         */
        Task<ClientRES> PostClient(ClientREQ request);



        /*
         *  PUT
         */
        Task<ClientRES> PutClient(ClientREQ request);


        #endregion

        #region Delivery
        /*
         *  GET
         */
        Task<IEnumerable<DeliveryDetailRES>> GetDeliveryDetails(DeliveryDetailsFilter filter);
        Task<IEnumerable<TrackRES>> GetTracks(TrackFilter filter);

        /*
         *  DELETE
         */
        Task<bool> DeleteTrack(int id);

        /*
         *  POST
         */
        Task<TrackRES> PostTrack(TrackREQ request);
        Task<DeliveryDetailRES> PostDeliveryDetail(DeliveryDetailREQ request);

        /*
         *  PUT
         */
        Task<TrackRES> PutTrack(TrackREQ request);
        Task<DeliveryDetailRES> PutDeliveryDetail(DeliveryDetailREQ request);

        #endregion

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
         *  DELETE
         */
        Task<bool> DeleteProductCategory(int id);
        Task<bool> DeleteProductBrand(int id);
        Task<bool> DeleteProductHallway(int id);
        Task<bool> DeleteProductLevel(int id);
        Task<bool> DeleteProductShelf(int id);
        Task<bool> DeleteProductKit(int id);


        /*
         *  POST
         */

        Task<GenericCatalog> PostProductCategory(GenericCatalogREQ request);
        Task<GenericCatalog> PostProductBrand(GenericCatalogREQ request);
        Task<GenericCatalog> PostProductKit(GenericCatalogREQ request);
        Task<GenericCatalog> PostProductHallway(GenericCatalogREQ request);
        Task<GenericCatalog> PostProductLevel(GenericCatalogREQ request);
        Task<GenericCatalog> PostProductShelf(GenericCatalogREQ request);

        /*
         *  PUT
         */
        Task<GenericCatalog> PutProductCategory(GenericCatalogREQ request);
        Task<GenericCatalog> PutProductBrand(GenericCatalogREQ request);
        Task<GenericCatalog> PutProductKit(GenericCatalogREQ request);
        Task<GenericCatalog> PutProductHallway(GenericCatalogREQ request);
        Task<GenericCatalog> PutProductLevel(GenericCatalogREQ request);
        Task<GenericCatalog> PutProductShelf(GenericCatalogREQ request);


        #endregion

        #region Sale
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
