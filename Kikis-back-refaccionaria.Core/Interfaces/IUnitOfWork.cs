using Kikis_back_refaccionaria.Core.Entities;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IUnitOfWork : IDisposable {

        IRepository<TbPermission> Permission { get; }
        IRepository<TbRol> Rol { get; }
        IRepository<TbTool> Tool { get; }
        IRepository<TbSale> Sale { get; }
        IRepository<TbSaleDetail> SaleDetail { get; }
        IRepository<TbSupplier> Supplier { get; }
        IRepository<TbToolBrand> ToolBrand { get; }
        IRepository<TbToolCategory> ToolCategory { get; }
        IRepository<TbUser> User { get; }



        Task SaveChangeAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
    }
}
