using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class UnitOfWork : IUnitOfWork {

        private readonly KikisDbContext _context;
        private IDbContextTransaction _transaction;

        private readonly IRepository<TbDeliveryDetail> _deliveryDetail;
        private readonly IRepository<TbKit> _kit;
        private readonly IRepository<TbRol> _rol;
        private readonly IRepository<TbPermission> _permission;
        private readonly IRepository<TbProduct> _product;
        private readonly IRepository<TbProductBrand> _productBrand;
        private readonly IRepository<TbProductCategory> _productCategory;
        private readonly IRepository<TbProductHallway> _productHallway;
        private readonly IRepository<TbProductKit> _productKit;
        private readonly IRepository<TbProductLevel> _productLevel;
        private readonly IRepository<TbProductShelf> _productShelf;
        private readonly IRepository<TbProductSupplier> _productSupplier;
        private readonly IRepository<TbSale> _sale;
        private readonly IRepository<TbSaleDetail> _saleDetail;
        private readonly IRepository<TbSupplier> _supplier;
        private readonly IRepository<TbTrackDelivery> _trackDeliveries;
        private readonly IRepository<TbTrack> _track;
        private readonly IRepository<TbUser> _user;

        public UnitOfWork(KikisDbContext context) {
            _context = context;
        }

        public IRepository<TbDeliveryDetail> DeliveryDetail => _deliveryDetail ?? new Repository<TbDeliveryDetail>(_context);
        public IRepository<TbKit> Kit => _kit ?? new Repository<TbKit>(_context);
        public IRepository<TbRol> Rol => _rol ?? new Repository<TbRol>(_context);
        public IRepository<TbPermission> Permission => _permission ?? new Repository<TbPermission>(_context);
        public IRepository<TbProduct> Product => _product ?? new Repository<TbProduct>(_context);
        public IRepository<TbProductBrand> ProductBrand => _productBrand ?? new Repository<TbProductBrand>(_context);
        public IRepository<TbProductCategory> ProductCategory => _productCategory ?? new Repository<TbProductCategory>(_context);
        public IRepository<TbProductHallway> ProductHallway => _productHallway ?? new Repository<TbProductHallway>(_context);
        public IRepository<TbProductKit> ProductKit => _productKit ?? new Repository<TbProductKit>(_context);
        public IRepository<TbProductLevel> ProductLevel => _productLevel ?? new Repository<TbProductLevel>(_context);
        public IRepository<TbProductShelf> ProductShelf => _productShelf ?? new Repository<TbProductShelf>(_context);
        public IRepository<TbProductSupplier> ProductSupplier => _productSupplier ?? new Repository<TbProductSupplier>(_context);
        public IRepository<TbSale> Sale => _sale ?? new Repository<TbSale>(_context);
        public IRepository<TbSaleDetail> SaleDetail => _saleDetail ?? new Repository<TbSaleDetail>(_context);
        public IRepository<TbSupplier> Supplier => _supplier ?? new Repository<TbSupplier>(_context);
        public IRepository<TbTrackDelivery> TrackDelivery => _trackDeliveries ?? new Repository<TbTrackDelivery>(_context);
        public IRepository<TbTrack> Track => _track ?? new Repository<TbTrack>(_context);
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
