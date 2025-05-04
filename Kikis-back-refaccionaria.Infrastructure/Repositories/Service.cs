using AutoMapper;
using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Kikis_back_refaccionaria.Infrastructure.Encryption;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class Service : IService {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public Service(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                password = Hashing.EncodeSHA256(password);

                var user = new TbUser {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Curp = request.Curp,
                    Rol = request.Rol.Id,
                    CreateDate = request.CreateDate,
                    Password = password,
                    IsActive = true,
                };

                _unitOfWork.User.Add(user);
                await _unitOfWork.SaveChangeAsync();

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

        #region Tool
        /*
         *  GET
         */
        public async Task<IEnumerable<ToolRES>> GetTools(ToolFilter filter) {

            //query
            var query = _unitOfWork.Tool
                .GetQuery()
                .Include(tool => tool.BrandNavigation)
                .Include(tool => tool.CategoryNavigation)
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var tools = await query.Select(x => new ToolRES {
                Id = x.Id,
                Name = x.Name,
                Barcode = x.Barcode,
                Brand = x.BrandNavigation.Name,
                Category = x.CategoryNavigation.Name,
                Quantity = x.Quantity,
                Price = x.Price,
                Discount = x.Discount,
                Path = x.Path,
                IsActive = x.IsActive
            }).ToListAsync();

            return tools;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteTool(int id) {

            try {

                var tool = await _unitOfWork.Tool.GetById(id);
                if(tool == null)
                    throw new BusinessException("Producto no encontrado");

                tool.IsActive = false;

                _unitOfWork.Tool.Update(tool);
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
        public async Task<ToolRES> PostTool(ToolREQ request) {

            try {

                var tool = _mapper.Map<TbTool>(request);

                _unitOfWork.Tool.Add(tool);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetTools(new ToolFilter { Id = tool.Id });
                var toolRES = lastInsert.FirstOrDefault();

                return toolRES;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar producto\n{ex.Message}");
            }
        }


        /*
         *  PUT
         */
        public async Task<bool> PutToolPromotion(ToolPromotionREQ request) {

            try {

                var tool = await _unitOfWork.Tool.GetById(request.Id);
                if(tool == null)
                    throw new BusinessException("Producto no encontrado");

                tool.Discount = request.Discount;

                _unitOfWork.Tool.Update(tool);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar promocion: {ex.Message}");
            }
        }
        public async Task<bool> PutToolStock(ToolStockREQ request) {

            try {

                var tool = await _unitOfWork.Tool.GetById(request.Id);
                if(tool == null)
                    throw new BusinessException("Producto no encontrado");

                tool.Quantity += request.StockToAdd;

                _unitOfWork.Tool.Update(tool);
                await _unitOfWork.SaveChangeAsync();

                return true;

            }
            catch(Exception ex) {

                throw new BusinessException($"Error al actualizar el stock: {ex.Message}");
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
        public async Task<IEnumerable<GenericCatalog>> GetToolCategory() {
            var data = await _unitOfWork.ToolCategory.GetAll();

            var response = data.Select(x => new GenericCatalog{
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }
        public async Task<IEnumerable<GenericCatalog>> GetToolBrand() {
            var data = await _unitOfWork.ToolBrand.GetAll();

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
                Iva = sale.Iva,
                Total = sale.Total,
                Pay = sale.Pay,
                CreateDate = sale.CreateDate,
                SaleDetails = sale.TbSaleDetails.Select(sale => new SaleDetail {
                    Id = sale.Id,
                    Tool = sale.Tool,
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
                    Iva = request.Iva,
                    Total = request.Total,
                    Pay = request.Pay,
                    CreateDate = request.CreateDate,
                };
                _unitOfWork.Sale.Add(sale);
                await _unitOfWork.SaveChangeAsync();

                var saleDatails = request.SaleDetails.Select(x => new TbSaleDetail {
                    Sale = sale.Id,
                    Tool = x.Tool,
                    Price = x.Price,
                    PriceUnit = x.PriceUnit,
                    Quantity = x.Quantity,
                    Total= x.Total,
                });
                _unitOfWork.SaleDetail.AddRange(saleDatails);
                await _unitOfWork.SaveChangeAsync();

                //update tool
                var toolSends = saleDatails
                    .GroupBy(s => s.Tool)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Quantity));
                var toolIds = toolSends.Keys.ToList();

                var tools = await _unitOfWork.Tool
                    .GetQuery()
                    .Where(x => toolIds.Contains(x.Id))
                    .ToListAsync();

                foreach(var t in tools) {
                    if(toolSends.TryGetValue(t.Id, out var quantitySend)) {
                        t.Quantity -= quantitySend;
                    }
                }
                _unitOfWork.Tool.UpdateRange(tools);
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
                Rfc = x.Rfc,
                Curp = x.Curp,
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
                    Rfc = supplier.Rfc,
                    Curp = supplier.Curp,
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
