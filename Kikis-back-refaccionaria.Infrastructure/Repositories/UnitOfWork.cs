using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class UnitOfWork : IUnitOfWork {

        private readonly KikisDbContext _context;
        private IDbContextTransaction _transaction;

        private readonly IRepository<TbRol> _rol;
        private readonly IRepository<TbPermission> _permission;
        private readonly IRepository<TbTool> _tool;
        private readonly IRepository<TbSale> _sale;
        private readonly IRepository<TbSaleDetail> _saleDetail;
        private readonly IRepository<TbSupplier> _supplier;
        private readonly IRepository<TbToolBrand> _toolBrand;
        private readonly IRepository<TbToolCategory> _toolCategory;
        private readonly IRepository<TbUser> _user;

        public UnitOfWork(KikisDbContext context) {
            _context = context;
        }

        public IRepository<TbRol> Rol => _rol ?? new Repository<TbRol>(_context);
        public IRepository<TbPermission> Permission => _permission ?? new Repository<TbPermission>(_context);
        public IRepository<TbSale> Sale => _sale ?? new Repository<TbSale>(_context);
        public IRepository<TbSaleDetail> SaleDetail => _saleDetail ?? new Repository<TbSaleDetail>(_context);
        public IRepository<TbSupplier> Supplier => _supplier ?? new Repository<TbSupplier>(_context);
        public IRepository<TbTool> Tool => _tool ?? new Repository<TbTool>(_context);
        public IRepository<TbToolBrand> ToolBrand => _toolBrand ?? new Repository<TbToolBrand>(_context);
        public IRepository<TbToolCategory> ToolCategory => _toolCategory ?? new Repository<TbToolCategory>(_context);
        public IRepository<TbUser> User => _user ?? new Repository<TbUser>(_context);


        public async Task SaveChangeAsync() {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync() {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync() {
            try {
                await _transaction.CommitAsync();
            }
            catch(Exception) {
                await _transaction.RollbackAsync();
            }
            finally {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public void Dispose() {
            _context.Dispose();
        }
    }
}
