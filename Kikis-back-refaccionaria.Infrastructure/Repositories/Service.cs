using AutoMapper;
using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Kikis_back_refaccionaria.Infrastructure.Encryption;
using Kikis_back_refaccionaria.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class Service : IService {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceEMail _emailService;
        public Service(IUnitOfWork unitOfWork, IMapper mapper, IServiceEMail emailService) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        #region User
        /*
         *  GET
         */
        public async Task<IEnumerable<RolRES>> GetRols(RolFilter filter) {
            //query
            var query = _unitOfWork.Permission
                .GetQuery()
                .Include(p => p.ModuleNavigation)
                .Include(p => p.RolNavigation)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Rol == filter.Id);

            var rols = await query.ToListAsync();

            // Agrupar
            var grouped = rols
                .GroupBy(p => new { p.RolNavigation.Id, p.RolNavigation.Name })
                .Select(g => new RolRES {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Modules = g.Select(p => new ModuleRES {
                        Id = p.ModuleNavigation.Id,
                        Name = p.ModuleNavigation.Name,
                        Description = p.ModuleNavigation.Description,
                        Permissions = new PermissionRES {
                            Id = p.Id,
                            CanAdd = p.CanAdd,
                            CanEdit = p.CanEdit,
                            CanDelete = p.CanDelete,
                            CanView = p.CanView
                        }
                    }).ToList()
                }).ToList();

            return grouped;
        }
        public async Task<IEnumerable<UserRES>> GetUsers(UserFilter filter) {
            
            //query
            var query = _unitOfWork.User
                .GetQuery()
                .Include(p => p.RolNavigation)
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var users = await query.Select(x => new UserRES {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Curp = x.Curp,
                Rol = new GenericCatalog {
                    Id = x.RolNavigation.Id,
                    Name = x.RolNavigation.Name,
                }
            }).ToListAsync();

            return users;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteUser(int id) {

            try {

                var user = await _unitOfWork.User.GetById(id);
                if(user == null)
                    throw new BusinessException("Usuario no encontrado");

                user.IsActive = false;

                _unitOfWork.User.Update(user);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al eliminar: {ex.Message}");
            }

        }


        /*
         *  POST
         */
        public async Task<UserRES> PostUser(UserREQ request) {

            try {
                string password = Utilities.Util.Generator(8);
                string passwordEncode = Hashing.EncodeSHA256(password);

                var user = new TbUser {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Curp = request.Curp,
                    Rol = request.Rol.Id,
                    CreateDate = request.CreateDate,
                    Password = passwordEncode,
                    IsActive = true,
                };

                _unitOfWork.User.Add(user);
                await _unitOfWork.SaveChangeAsync();

                _emailService.SendUserPasswordEmail(user.Email, password);

                var lastInsert = await GetUsers(new UserFilter { Id = user.Id });
                var response = lastInsert.FirstOrDefault();

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar usuario\n{ex.Message}");
            }
        }
        public async Task<UserAuthRES> Login(AuthREQ request) {

            try {
                request.Password = Hashing.EncodeSHA256(request.Password);

                var user = _unitOfWork.User
                    .GetQuery()
                    .FirstOrDefault(x =>
                        x.Email == request.Email &&
                        x.Password == request.Password);

                if(user == null)
                    throw new BusinessException("Usuario no encontrado, Revise sus credenciales");

                if(user.IsActive == false)
                    throw new BusinessException("La cuenta del usuario fue desactivada");

                //select
                var rol = await GetRols(new RolFilter { Id = user.Rol });
                var response = new UserAuthRES {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Curp = user.Curp,
                };
                response.Rol = rol.FirstOrDefault();

                return response;

            }
            catch(Exception ex) {
                throw new BusinessException($"Ocurrió un error inesperado al intentar iniciar sesión\n{ex.Message}");
            }
        }
        public async Task<bool> PostRols(RolRES request) {

            try {
                await _unitOfWork.BeginTransactionAsync();

                //rol
                var rol = new TbRol();
                rol.Name = request.Name;
                _unitOfWork.Rol.Add(rol);
                await _unitOfWork.SaveChangeAsync();

                //permission
                var permissions = request.Modules.Select(p => new TbPermission {
                    Rol = rol.Id,
                    Module = p.Id,
                    CanAdd = p.Permissions.CanAdd,
                    CanEdit = p.Permissions.CanEdit,
                    CanDelete = p.Permissions.CanDelete,
                    CanView = p.Permissions.CanView,
                });
                _unitOfWork.Permission.AddRange(permissions);
                await _unitOfWork.SaveChangeAsync();

                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch(Exception ex) {
                throw new BusinessException($"Ocurrió un error inesperado al agregar rol\n{ex.Message}");
            }
            
        }

        /*
         *  PUT
         */
        public async Task<bool> PutRols(RolRES request) {

            try {
                await _unitOfWork.BeginTransactionAsync();

                //rol
                var rol = await _unitOfWork.Rol.GetById(request.Id);
                rol.Name = request.Name;
                _unitOfWork.Rol.Update(rol);
                await _unitOfWork.SaveChangeAsync();

                //permission
                var permissions = await _unitOfWork.Permission
                    .GetQuery()
                    .Where(x => x.Rol == request.Id)
                    .ToListAsync();

                foreach(var module in request.Modules) {

                    var permission = permissions.FirstOrDefault(x => x.Id == module.Permissions.Id);
                    if(permission != null) {

                        permission.CanAdd = module.Permissions.CanAdd;
                        permission.CanEdit = module.Permissions.CanEdit;
                        permission.CanDelete = module.Permissions.CanDelete;
                        permission.CanView = module.Permissions.CanView;

                        _unitOfWork.Permission.Update(permission);
                    }
                }
                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch(Exception ex) {
                throw new BusinessException($"Ocurrió un error inesperado al agregar rol\n{ex.Message}");
            }

        }
        public async Task<UserRES> PutUser(UserREQ request) {

            try {

                var user = await _unitOfWork.User.GetById(request.Id);
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Curp = request.Curp;
                user.Rol = request.Rol.Id;

                _unitOfWork.User.Update(user);
                await _unitOfWork.SaveChangeAsync();

                var response = new UserRES {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Curp = request.Curp,
                    Rol = new GenericCatalog {
                        Id = request.Rol.Id,
                        Name = request.Rol.Name,
                    }
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar usuario\n{ex.Message}");
            }
        }


        #endregion

        #region Product
        /*
         *  GET
         */
        public async Task<IEnumerable<ProductRES>> GetProducts(ProductFilter filter) {

            //query
            var query = _unitOfWork.Product
                .GetQuery()
                .Include(product => product.BrandNavigation)
                .Include(product => product.CategoryNavigation)
                .Include(product => product.HallwayNavigation)
                .Include(product => product.LevelNavigation)
                .Include(product => product.ShelfNavigation)
                .Include(product => product.TbProductSuppliers)
                    .ThenInclude(product => product.SupplierNavigation)
                .Include(product => product.TbProductKits)
                    .ThenInclude(product => product.KitNavigation)
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var products = await query.Select(x => new ProductRES {
                Id = x.Id,
                Name = x.Name,
                Barcode = x.Barcode,
                Brand = x.BrandNavigation.Name,
                Category = x.CategoryNavigation.Name,
                Hallway = new GenericCatalog {
                    Id = x.HallwayNavigation.Id,
                    Name = x.HallwayNavigation.Name,
                },
                Level = new GenericCatalog {
                    Id = x.LevelNavigation.Id,
                    Name = x.LevelNavigation.Name,
                },
                Shelf = new GenericCatalog {
                    Id = x.ShelfNavigation.Id,
                    Name = x.ShelfNavigation.Name,
                },
                Suppliers = x.TbProductSuppliers.Select(x => new SupplierRES {
                    Id = x.SupplierNavigation.Id,
                    BusinessName = x.SupplierNavigation.BusinessName,
                }).ToList(),
                Kits = x.TbProductKits.Select(x => new GenericCatalog {
                    Id = x.KitNavigation.Id,
                    Name= x.KitNavigation.Name,
                }).ToList(),
                Quantity = x.Quantity,
                Price = x.Price,
                Discount = x.Discount,
                Path = Constants.HOST + x.Path,
                IsActive = x.IsActive
            }).ToListAsync();



            return products;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteProduct(int id) {

            try {

                var Product = await _unitOfWork.Product.GetById(id);
                if(Product == null)
                    throw new BusinessException("Producto no encontrado");

                Product.IsActive = false;

                _unitOfWork.Product.Update(Product);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al eliminar: {ex.Message}");
            }
        }


        /*
         *  POST
         */
        public async Task<ProductRES> PostProduct(ProductREQ request) {

            try {

                var product = _mapper.Map<TbProduct>(request);

                //product
                product.Path = Util.SaveBase64File(request, Constants.PATH_IMG_PRODUCT);
                _unitOfWork.Product.Add(product);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetProducts(new ProductFilter { Id = product.Id });
                var productRES = lastInsert.FirstOrDefault();


                //supplier
                var suppliers = request.Suppliers.Select(x => new TbProductSupplier {
                    Product = product.Id,
                    Supplier = x.Id
                });
                _unitOfWork.ProductSupplier.AddRange(suppliers);
                await _unitOfWork.SaveChangeAsync();


                //kits
                var kits = request.Kits.Select(x => new TbProductKit {
                    Product = product.Id,
                    Kit = x.Id
                });
                _unitOfWork.ProductKit.AddRange(kits);
                await _unitOfWork.SaveChangeAsync();


                //mapper supplier
                productRES.Suppliers = request.Suppliers.Select(x => new SupplierRES {
                    Id = x.Id,
                    BusinessName = x.BusinessName
                }).ToList();
                productRES.Kits = request.Kits.Select(x => new GenericCatalog {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                return productRES;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar producto\n{ex.Message}");
            }
        }


        /*
         *  PUT
         */
        public async Task<bool> PutProductPromotion(ProductPromotionREQ request) {

            try {

                var Product = await _unitOfWork.Product.GetById(request.Id);
                if(Product == null)
                    throw new BusinessException("Producto no encontrado");

                Product.Discount = request.Discount;

                _unitOfWork.Product.Update(Product);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar promocion: {ex.Message}");
            }
        }
        public async Task<bool> PutProductStock(ProductStockREQ request) {

            try {

                var Product = await _unitOfWork.Product.GetById(request.Id);
                if(Product == null)
                    throw new BusinessException("Producto no encontrado");

                Product.Quantity += request.StockToAdd;

                _unitOfWork.Product.Update(Product);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar el stock: {ex.Message}");
            }
        }
        public async Task<bool> PutProductWarehouse(ProductWarehouseREQ request) {

            try {

                var product = await _unitOfWork.Product.GetById(request.Id);
                if(product == null)
                    throw new BusinessException("Producto no encontrado");

                product.Hallway = request.Hallway.Id;
                product.Level = request.Level.Id;
                product.Shelf = request.Shelf.Id;

                _unitOfWork.Product.Update(product);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar la ubicacion del almacen: {ex.Message}");
            }
        }
        public async Task<bool> PutProductSupplier(ProductRES request) {

            try {
                await _unitOfWork.BeginTransactionAsync();

                var supplierSave = await _unitOfWork.ProductSupplier
                    .GetQuery()
                    .Where(x => x.Product == request.Id)
                    .ToListAsync();

                var supplierMap = request.Suppliers.Select(x => new TbProductSupplier {
                    Id = 0,
                    Product = request.Id,
                    Supplier = x.Id,
                }).ToList();

                //Deletes
                var removed = supplierSave.Where(x => !supplierMap.Any(s => s.Supplier == x.Supplier))
                    .ToList();
                if(removed.Any()) {

                    _unitOfWork.ProductSupplier.DeleteRange(removed);
                    await _unitOfWork.SaveChangeAsync();
                }

                //Adds
                var news = supplierMap.Where(x => !supplierSave.Any(s => s.Supplier == x.Supplier))
                    .ToList();
                if(news.Any()) {

                    _unitOfWork.ProductSupplier.AddRange(news);
                    await _unitOfWork.SaveChangeAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar la proveedor: {ex.Message}");
            }
        }
        public async Task<bool> PutProductKit(ProductRES request) {
            try {

                await _unitOfWork.BeginTransactionAsync();

                var kitSave = await _unitOfWork.ProductKit
                    .GetQuery()
                    .Where(x => x.Product == request.Id)
                    .ToListAsync();

                var kitMap = request.Kits.Select(x => new TbProductKit {
                    Id = 0,
                    Product = request.Id,
                    Kit = x.Id,
                }).ToList();

                //Deletes
                var removed = kitSave.Where(x => !kitMap.Any(s => s.Kit == x.Kit))
                    .ToList();
                if(removed.Any()) {

                    _unitOfWork.ProductKit.DeleteRange(removed);
                    await _unitOfWork.SaveChangeAsync();
                }

                //Adds
                var news = kitMap.Where(x => !kitSave.Any(s => s.Kit == x.Kit))
                    .ToList();
                if(news.Any()) {

                    _unitOfWork.ProductKit.AddRange(news);
                    await _unitOfWork.SaveChangeAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar el kit (grupo): {ex.Message}");
            }
        }


        /*
         *  PUT
         */
        #endregion

        #region Catalogs
        /*
         *  GET
         */
        public async Task<IEnumerable<GenericCatalog>> GetProductCategory() {
            var data = await _unitOfWork.ProductCategory.GetAll();

            var response = data.Select(x => new GenericCatalog{
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }
        
        public async Task<IEnumerable<GenericCatalog>> GetProductBrand() {
            var data = await _unitOfWork.ProductBrand.GetAll();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductHallway(){

            var data = await _unitOfWork.ProductHallway.GetAll();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductLevel(){

            var data = await _unitOfWork.ProductLevel.GetAll();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductShelf() {

            var data = await _unitOfWork.ProductShelf.GetAll();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductKit() {

            var data = await _unitOfWork.Kit.GetAll();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }


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
        public async Task<IEnumerable<SaleRES>> GetSales(SaleFilter filter) {

            //query
            var query = _unitOfWork.Sale
                .GetQuery()
                .Include(sale => sale.SellerNavigation)
                .Include(sale => sale.TbSaleDetails)
                    .ThenInclude(sale => sale.ProductNavigation)
                .AsNoTracking();
            var products = _unitOfWork.Product
                .GetQuery()
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var sales = await query.Select(sale => new SaleRES {
                Id = sale.Id,
                Seller = new GenericCatalog {
                    Id = sale.SellerNavigation.Id,
                    Name = $"{sale.SellerNavigation.FirstName} {sale.SellerNavigation.LastName}"
                },
                SubTotal = sale.SubTotal,
                Iva = sale.IVA,
                Total = sale.Total,
                Pay = sale.Pay,
                CreateDate = sale.CreateDate,
                SaleDetails = sale.TbSaleDetails.Select(sale => new SaleDetail {
                    Id = sale.Id,
                    Product = sale.Product,
                    Name = sale.ProductNavigation.Name,
                    Price = sale.Price,
                    PriceUnit = sale.PriceUnit,
                    Quantity = sale.Quantity,
                    Total = sale.Total,
                }).ToList()
            }).ToListAsync();

            return sales;
        }


        /*
         *  POST
         */
        public async Task<bool> PostSales(SaleREQ request) {


            try {
                await _unitOfWork.BeginTransactionAsync();

                //add sale
                var sale = new TbSale {
                    Seller = request.Seller.Id,
                    SubTotal = request.SubTotal,
                    IVA = request.Iva,
                    Total = request.Total,
                    Pay = request.Pay,
                    CreateDate = request.CreateDate,
                };
                _unitOfWork.Sale.Add(sale);
                await _unitOfWork.SaveChangeAsync();

                var saleDatails = request.SaleDetails.Select(x => new TbSaleDetail {
                    Sale = sale.Id,
                    Product = x.Product,
                    Price = x.Price,
                    PriceUnit = x.PriceUnit,
                    Quantity = x.Quantity,
                    Total= x.Total,
                });
                _unitOfWork.SaleDetail.AddRange(saleDatails);
                await _unitOfWork.SaveChangeAsync();

                //update Product
                var ProductSends = saleDatails
                    .GroupBy(s => s.Product)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Quantity));
                var ProductIds = ProductSends.Keys.ToList();

                var Products = await _unitOfWork.Product
                    .GetQuery()
                    .Where(x => ProductIds.Contains(x.Id))
                    .ToListAsync();

                foreach(var t in Products) {
                    if(ProductSends.TryGetValue(t.Id, out var quantitySend)) {
                        t.Quantity -= quantitySend;
                    }
                }
                _unitOfWork.Product.UpdateRange(Products);
                await _unitOfWork.SaveChangeAsync();


                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar guardar la compra\n{ex.Message}");
            }
        }


        /*
         *  PUT
         */
        #endregion

        #region Supplier
        /*
         *  GET
         */
        public async Task<IEnumerable<SupplierRES>> GetSupplier() {

            var supplier = await _unitOfWork.Supplier.GetAll();

            var response = supplier.Select(x => new SupplierRES {

                Id = x.Id,
                BusinessName = x.BusinessName,
                TradeName = x.TradeName,
                Rfc = x.RFC,
                Curp = x.CURP,
                Email = x.Email,
                Cellphone = x.Cellphone,
                Cellphone2 = x.Cellphone2,
                Address = x.Address,
                Owner = x.Owner,
                Representative = x.Representative,
                IsActive = x.IsActive,
            }).ToList();

            return response;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteSupplier(int id) {

            try {

                var supplier = await _unitOfWork.Supplier.GetById(id);
                if(supplier == null)
                    throw new BusinessException("Proveedor no encontrado");

                supplier.IsActive = false;

                _unitOfWork.Supplier.Update(supplier);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al eliminar: {ex.Message}");
            }
        }


        /*
         *  POST
         */
        public async Task<SupplierRES> PostSupplier(SupplierRES request) {

            try {

                var supplier = new TbSupplier {
                    Id = request.Id,
                    BusinessName = request.BusinessName,
                    TradeName = request.TradeName,
                    RFC = request.Rfc,
                    CURP = request.Curp,
                    Email = request.Email,
                    Cellphone = request.Cellphone,
                    Cellphone2 = request.Cellphone2,
                    Address = request.Address,
                    Owner = request.Owner,
                    Representative = request.Representative,
                    IsActive = request.IsActive,
                };

                _unitOfWork.Supplier.Add(supplier);
                await _unitOfWork.SaveChangeAsync();

                var response = new SupplierRES {
                    Id = supplier.Id,
                    BusinessName = request.BusinessName,
                    TradeName = request.TradeName,
                    Rfc = request.Rfc,
                    Curp = request.Curp,
                    Email = request.Email,
                    Cellphone = request.Cellphone,
                    Cellphone2 = request.Cellphone2,
                    Address = request.Address,
                    Owner = request.Owner,
                    Representative = request.Representative,
                    IsActive = request.IsActive,
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar proveedor\n{ex.Message}");
            }
        }


        /*
         *  PUT
         */
        public async Task<SupplierRES> PutSupplier(SupplierRES request) {

            try {

                var supplier = await _unitOfWork.Supplier.GetById(request.Id);

                if(supplier == null)
                    throw new BusinessException("Proveedor no encontrado");

                supplier.TradeName = request.TradeName;
                supplier.Representative = request.Representative;
                supplier.Cellphone = request.Cellphone;
                supplier.Cellphone2 = request.Cellphone2;
                supplier.Email = request.Email;
                supplier.Address = request.Address;

                _unitOfWork.Supplier.Update(supplier);
                await _unitOfWork.SaveChangeAsync();

                var response = new SupplierRES {
                    Id = supplier.Id,
                    BusinessName = request.BusinessName,
                    TradeName = supplier.TradeName,
                    Rfc = supplier.RFC,
                    Curp = supplier.CURP,
                    Email = request.Email,
                    Cellphone = request.Cellphone,
                    Cellphone2 = request.Cellphone2,
                    Address = request.Address,
                    Owner = supplier.Owner,
                    Representative = request.Representative,
                    IsActive = supplier.IsActive,
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar proveedor\n{ex.Message}");
            }
        }

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
//dotnet ef dbcontext scaffold "server=127.0.0.1;port=3306;user=root;password=password;database=kikis_ferreteria" Pomelo.EntityFrameworkCore.MySql -o Models --context TuDbContext --context-dir Context --use-database-names --no-onconfiguring --data-annotations