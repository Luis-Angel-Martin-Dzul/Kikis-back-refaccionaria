using Kikis_back_refaccionaria.Core.Entities;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IUnitOfWork : IDisposable {

        IRepository<TbKit> Kit { get; }
        IRepository<TbPermission> Permission { get; }
        IRepository<TbProduct> Product { get; }
        IRepository<TbProductBrand> ProductBrand { get; }
        IRepository<TbProductCategory> ProductCategory { get; }
        IRepository<TbProductHallway> ProductHallway { get; }
        IRepository<TbProductKit> ProductKit { get; }
        IRepository<TbProductLevel> ProductLevel { get; }
        IRepository<TbProductShelf> ProductShelf { get; }
        IRepository<TbProductSupplier> ProductSupplier { get; }
        IRepository<TbRol> Rol { get; }
        IRepository<TbSale> Sale { get; }
        IRepository<TbSaleDetail> SaleDetail { get; }
        IRepository<TbSupplier> Supplier { get; }
        IRepository<TbUser> User { get; }



        Task SaveChangeAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
    }
}
