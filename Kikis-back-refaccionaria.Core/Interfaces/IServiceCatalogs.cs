using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceCatalogs {

        /*
         *  GET
         */
        Task<PagedResponse<GenericCatalog>> GetProductCategory(PaginationFilter filter);
        Task<PagedResponse<GenericCatalog>> GetProductBrand(PaginationFilter filter);
        Task<PagedResponse<GenericCatalog>> GetProductHallway(PaginationFilter filter);
        Task<PagedResponse<GenericCatalog>> GetProductLevel(PaginationFilter filter);
        Task<PagedResponse<GenericCatalog>> GetProductShelf(PaginationFilter filter);
        Task<PagedResponse<GenericCatalog>> GetProductKit(PaginationFilter filter);

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
    }
}
