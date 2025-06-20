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

        #region Delivery
        /*
         *  GET
         */
        public async Task<IEnumerable<DeliveryDetailRES>> GetDeliveryDetails(DeliveryDetailsFilter filter) {

            //query
            var query = _unitOfWork.DeliveryDetail
                .GetQuery()
                .Include(x => x.StatusNavigation)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);
            if(filter.Status != null)
                query = query.Where(x => x.Status == filter.Status);

            //select
            var response = await query.Select(x => new DeliveryDetailRES {
                Id = x.Id,
                Sale = x.Sale,
                Responsible = x.Responsible,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Status = new GenericCatalog {
                    Id = x.StatusNavigation.Id,
                    Name = x.StatusNavigation.Name,
                },
                Comments = x.Comments,
                CreateDate = x.CreateDate,
            }).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<TrackRES>> GetTracks(TrackFilter filter) {

            //query
            var query = _unitOfWork.Track
                .GetQuery()
                .Include(x => x.StatusNavigation)
                .Include(x => x.UserNavigation)
                .Include(x => x.TbTrackDeliveries)
                    .ThenInclude(td => td.DeliveryNavigation)
                    .ThenInclude(d => d.StatusNavigation)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            query = query.Where(x => x.IsActive == true);

            //select
            var response = await query.Select(x => new TrackRES {
                Id = x.Id,
                Name = x.Name,
                User = new UserREQ {
                    Id = x.UserNavigation.Id,
                    FirstName = x.UserNavigation.FirstName,
                    LastName = x.UserNavigation.LastName,
                    Email = x.UserNavigation.Email,
                    Curp = x.UserNavigation.Curp,
                    CreateDate = x.UserNavigation.CreateDate,
                },
                CreateDate = x.CreateDate,
                Status = new GenericCatalog {
                    Id = x.StatusNavigation.Id,
                    Name = x.StatusNavigation.Name,
                },
                Deliveries = x.TbTrackDeliveries.Select(td => new DeliveryDetailRES {
                    Id = td.DeliveryNavigation.Id,
                    Sale = td.DeliveryNavigation.Sale,
                    Responsible = td.DeliveryNavigation.Responsible,
                    Address = td.DeliveryNavigation.Address,
                    Latitude = td.DeliveryNavigation.Latitude,
                    Longitude = td.DeliveryNavigation.Longitude,
                    Comments = td.DeliveryNavigation.Comments ?? "",
                    CreateDate = td.DeliveryNavigation.CreateDate,
                    Status = new GenericCatalog {
                        Id = td.DeliveryNavigation.StatusNavigation.Id,
                        Name = td.DeliveryNavigation.StatusNavigation.Name
                    }
                }).ToList(),
                IsActive = x.IsActive
                
            }).ToListAsync();

            return response;
        }



        /*
         *  DELETE
         */
        public async Task<bool> DeleteTrack(int id) {

            try {

                var track = await _unitOfWork.Track.GetById(id);
                if(track == null)
                    throw new BusinessException("Ruta no encontrada");

                track.IsActive = false;

                _unitOfWork.Track.Update(track);
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
        public async Task<TrackRES> PostTrack(TrackREQ request) {

            try {
                await _unitOfWork.BeginTransactionAsync();


                //track
                var track = new TbTrack {
                    User = request.User,
                    Name = request.Name,
                    CreateDate = request.CreateDate,
                    Status = request.Deliveries.Count >= 1 ? 2 : 1,
                    IsActive = true,
                };

                _unitOfWork.Track.Add(track);
                await _unitOfWork.SaveChangeAsync();


                //track delivery
                var deliveries = request.Deliveries.Select(x => new TbTrackDelivery {

                    Track = track.Id,
                    Delivery = x.Delivery,
                });
                _unitOfWork.TrackDelivery.AddRange(deliveries);
                await _unitOfWork.SaveChangeAsync();


                //delivery update
                var deliveryIds = request.Deliveries.Select(d => d.Delivery).ToList();
                var deliveryDetails = await _unitOfWork.DeliveryDetail
                    .GetQuery()
                    .Where(d => deliveryIds.Contains(d.Id))
                    .ToListAsync();

                foreach(var delivery in deliveryDetails) {
                    delivery.Status = 2; //Pendiente
                }

                _unitOfWork.DeliveryDetail.UpdateRange(deliveryDetails);
                await _unitOfWork.SaveChangeAsync();



                //response
                var lastInsert = await GetTracks(new TrackFilter { Id = track.Id });
                var response = lastInsert.FirstOrDefault();

                await _unitOfWork.CommitTransactionAsync();
                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar ruta\n{ex.Message}");
            }
        }
        public async Task<DeliveryDetailRES> PostDeliveryDetail(DeliveryDetailREQ request) {
            try {

                var delivery = new TbDeliveryDetail {

                    Sale = request.Sale,
                    Responsible = request.Responsible,
                    Address = request.Address,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Status = request.Status.Id,
                    Comments = request.Comments,
                    CreateDate = request.CreateDate,
                };

                _unitOfWork.DeliveryDetail.Add(delivery);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetDeliveryDetails(new DeliveryDetailsFilter { Id = delivery.Id });
                var response = lastInsert.FirstOrDefault();

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar entrega\n{ex.Message}");
            }
        }

        /*
         *  PUT
         */
        public async Task<TrackRES> PutTrack(TrackREQ request) {

            try {
                await _unitOfWork.BeginTransactionAsync();

                //track
                var track = await _unitOfWork.Track.GetById((int)request.Id);
                track.Name = request.Name;
                track.User = request.User;

                _unitOfWork.Track.Update(track);
                await _unitOfWork.SaveChangeAsync();

                //track delivery
                var trackDeliverySave = await _unitOfWork.TrackDelivery
                    .GetQuery()
                    .Where(x => x.Track == request.Id)
                    .ToListAsync();

                var trackDeliveryMap = request.Deliveries.Select(x => new TbTrackDelivery {
                    Track = (int)request.Id,
                    Delivery = x.Delivery,
                }).ToList();

                //deletes
                var removed = trackDeliverySave.Where(x => !trackDeliveryMap.Any(s => s.Delivery == x.Delivery)).ToList();
                if(removed.Any()) {
                
                    _unitOfWork.TrackDelivery.DeleteRange(removed);
                    await _unitOfWork.SaveChangeAsync();
                }

                //adds
                var news = trackDeliveryMap.Where(x => !trackDeliverySave.Any(s => s.Delivery == x.Delivery)).ToList();
                if(news.Any()) {

                    _unitOfWork.TrackDelivery.AddRange(news);
                    await _unitOfWork.SaveChangeAsync();
                }

                //update
                var dDeliveryIds = removed.Select(d => d.Delivery).ToList();
                var dDeliveryDetails = await _unitOfWork.DeliveryDetail
                    .GetQuery()
                    .Where(d => dDeliveryIds.Contains(d.Id))
                    .ToListAsync();
                foreach(var delivery in dDeliveryDetails) delivery.Status = 1; //Creado
                
                _unitOfWork.DeliveryDetail.UpdateRange(dDeliveryDetails);


                var AdeliveryIds = news.Select(d => d.Delivery).ToList();
                var AdeliveryDetails = await _unitOfWork.DeliveryDetail
                    .GetQuery()
                    .Where(d => AdeliveryIds.Contains(d.Id))
                    .ToListAsync();
                foreach(var delivery in AdeliveryDetails) delivery.Status = 2; //Pendiente
                
                _unitOfWork.DeliveryDetail.UpdateRange(AdeliveryDetails);
                await _unitOfWork.SaveChangeAsync();



                var lastInsert = await GetTracks(new TrackFilter { Id = track.Id });
                var response = lastInsert.FirstOrDefault();

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar entrega\n{ex.Message}");
            }
        }
        public async Task<DeliveryDetailRES> PutDeliveryDetail(DeliveryDetailREQ request) {
            try {

                var delivery = new TbDeliveryDetail {
                    Id = (int)request.Id,
                    Sale = request.Sale,
                    Responsible = request.Responsible,
                    Address = request.Address,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Status = request.Status.Id,
                    Comments = request.Comments,
                    CreateDate = request.CreateDate,
                };

                _unitOfWork.DeliveryDetail.Update(delivery);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetDeliveryDetails(new DeliveryDetailsFilter { Id = delivery.Id });
                var response = lastInsert.FirstOrDefault();

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar entrega\n{ex.Message}");
            }
        }
        #endregion

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
            if(filter.Roles != null && filter.Roles.Any())
                query = query.Where(x => filter.Roles.Contains(x.Rol));

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
            var data = _unitOfWork.ProductCategory
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = await data.Select(x => new GenericCatalog{
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

            return response;
        }
        
        public async Task<IEnumerable<GenericCatalog>> GetProductBrand() {
            var data = _unitOfWork.ProductBrand
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = await data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductHallway(){

            var data = _unitOfWork.ProductHallway
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductLevel(){

            var data = _unitOfWork.ProductLevel
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductShelf() {

            var data = _unitOfWork.ProductShelf
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<GenericCatalog>> GetProductKit() {

            var data = _unitOfWork.Kit
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            var response = data.Select(x => new GenericCatalog {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return response;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteProductCategory(int id) {

            var category = await _unitOfWork.ProductCategory
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(category == null)
                throw new BusinessException("Categoria no encontrado");

            if(category.TbProducts.Count > 0)
                throw new BusinessException("La categoría no puede eliminarse ya que está siendo utilizada");

            category.IsActive = false;

            _unitOfWork.ProductCategory.Update(category);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> DeleteProductBrand(int id) {

            var brand = await _unitOfWork.ProductBrand
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(brand == null)
                throw new BusinessException("Marca no encontrada");

            if(brand.TbProducts.Count > 0)
                throw new BusinessException("La marca no puede eliminarse ya que está siendo utilizada");

            brand.IsActive = false;

            _unitOfWork.ProductBrand.Update(brand);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> DeleteProductHallway(int id) {

            var hallway = await _unitOfWork.ProductHallway
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(hallway == null)
                throw new BusinessException("Pasillo no encontrada");

            if(hallway.TbProducts.Count > 0)
                throw new BusinessException("El pasillo no puede eliminarse ya que está siendo utilizado");

            hallway.IsActive = false;

            _unitOfWork.ProductHallway.Update(hallway);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> DeleteProductLevel(int id) {

            var level = await _unitOfWork.ProductLevel
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(level == null)
                throw new BusinessException("Nivel no encontrado");

            if(level.TbProducts.Count > 0)
                throw new BusinessException("El nivel no puede eliminarse ya que está siendo utilizado");

            level.IsActive = false;

            _unitOfWork.ProductLevel.Update(level);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> DeleteProductShelf(int id) {

            var shelf = await _unitOfWork.ProductShelf
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(shelf == null)
                throw new BusinessException("Estante no encontrada");

            if(shelf.TbProducts.Count > 0)
                throw new BusinessException("El estante no puede eliminarse ya que está siendo utilizado");

            shelf.IsActive = false;

            _unitOfWork.ProductShelf.Update(shelf);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> DeleteProductKit(int id) {

            var kit = await _unitOfWork.Kit
                .GetQuery()
                .Where(x => x.Id == id)
                .Include(x => x.TbProductKits)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(kit == null)
                throw new BusinessException("Kit no encontrado");

            if(kit.TbProductKits.Count > 0)
                throw new BusinessException("El kit no puede eliminarse ya que está siendo utilizado");

            kit.IsActive = false;

            _unitOfWork.Kit.Update(kit);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }


        /*
         *  POST
         */
        public async Task<GenericCatalog> PostProductCategory(GenericCatalogREQ request) {


            try {

                var category = new TbProductCategory {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.ProductCategory.Add(category);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar categoria\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PostProductBrand(GenericCatalogREQ request) {


            try {

                var brand = new TbProductBrand {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.ProductBrand.Add(brand);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar marca\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PostProductKit(GenericCatalogREQ request) {

            try {

                var kit = new TbKit {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.Kit.Add(kit);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = kit.Id,
                    Name = kit.Name,
                    Description = kit.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar kit\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PostProductHallway(GenericCatalogREQ request) {

            try {

                var hallway = new TbProductHallway {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.ProductHallway.Add(hallway);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = hallway.Id,
                    Name = hallway.Name,
                    Description = hallway.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar pasillo\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PostProductLevel(GenericCatalogREQ request) {

            try {

                var level = new TbProductLevel {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.ProductLevel.Add(level);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = level.Id,
                    Name = level.Name,
                    Description = level.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar nivel\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PostProductShelf(GenericCatalogREQ request) {

            try {

                var shelf = new TbProductShelf {
                    Name = request.Name,
                    Description = request.Description
                };

                _unitOfWork.ProductShelf.Add(shelf);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog() {
                    Id = shelf.Id,
                    Name = shelf.Name,
                    Description = shelf.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar estante\n{ex.Message}");
            }
        }



        /*
         *  PUT
         */
        public async Task<GenericCatalog> PutProductCategory(GenericCatalogREQ request) {


            try {

                var category = await _unitOfWork.ProductCategory.GetById((int)request.Id);
                if(category == null)
                    throw new BusinessException("Categoria no encontrado");

                category.Name = request.Name;
                category.Description = request.Description;

                _unitOfWork.ProductCategory.Update(category);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar categoria\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PutProductBrand(GenericCatalogREQ request) {


            try {

                var brand = await _unitOfWork.ProductBrand.GetById((int)request.Id);
                if(brand == null)
                    throw new BusinessException("Marca no encontrado");

                brand.Name = request.Name;
                brand.Description = request.Description;

                _unitOfWork.ProductBrand.Update(brand);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar marca\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PutProductKit(GenericCatalogREQ request) {

            try {

                var kit = await _unitOfWork.Kit.GetById((int)request.Id);
                if(kit == null)
                    throw new BusinessException("Kit no encontrado");

                kit.Name = request.Name;
                kit.Description = request.Description;

                _unitOfWork.Kit.Update(kit);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = kit.Id,
                    Name = kit.Name,
                    Description = kit.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar kit\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PutProductHallway(GenericCatalogREQ request) {

            try {

                var hallway = await _unitOfWork.ProductHallway.GetById((int)request.Id);
                if(hallway == null)
                    throw new BusinessException("Pasillo no encontrado");

                hallway.Name = request.Name;
                hallway.Description = request.Description;

                _unitOfWork.ProductHallway.Update(hallway);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = hallway.Id,
                    Name = hallway.Name,
                    Description = hallway.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar pasillo\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PutProductLevel(GenericCatalogREQ request) {

            try {

                var level = await _unitOfWork.ProductLevel.GetById((int)request.Id);
                if(level == null)
                    throw new BusinessException("Nivel no encontrado");

                level.Name = request.Name;
                level.Description = request.Description;

                _unitOfWork.ProductLevel.Update(level);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = level.Id,
                    Name = level.Name,
                    Description = level.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar nivel\n{ex.Message}");
            }
        }
        public async Task<GenericCatalog> PutProductShelf(GenericCatalogREQ request) {

            try {

                var shelf = await _unitOfWork.ProductShelf.GetById((int)request.Id);
                if(shelf == null)
                    throw new BusinessException("Estante no encontrado");

                shelf.Name = request.Name;
                shelf.Description = request.Description;

                _unitOfWork.ProductShelf.Update(shelf);
                await _unitOfWork.SaveChangeAsync();

                var response = new GenericCatalog {
                    Id = shelf.Id,
                    Name = shelf.Name,
                    Description = shelf.Description
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar estante\n{ex.Message}");
            }
        }

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
                .Include(sale => sale.TbInvoices)
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
                }).ToList(),
                Invoice = sale.TbInvoices.Count() == 0 ? 0 : sale.TbInvoices.FirstOrDefault().Id
            }).ToListAsync();

            return sales;
        }

        public async Task<IEnumerable<InvoiceRES>> GetInvoices(InvoiceFilter filter) {

            //query
            var query = _unitOfWork.Invoice
                .GetQuery()
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            //select
            var invoices = await query.Select(invoice => new InvoiceRES {
                
                Id = invoice.Id,
                Sale = invoice.Sale,
                Name = invoice.Name,
                RFC = invoice.RFC,
                CodePostal = invoice.CodePostal,
                Contact = invoice.Contact,
                TaxRegime = invoice.TaxRegime,
                TaxRegimeName = invoice.TaxRegimeName,
                UseCFDI = invoice.UseCFDI,
                UseCFDIName = invoice.UseCFDIName,
                CreateDate = invoice.CreateDate,
            }).ToListAsync();

            return invoices;
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
        public async Task<int> PostInvoice(InvoiceREQ request) {

            try {

                //add sale
                var invoice = new TbInvoice {
                    Sale = request.Sale,
                    Name = request.Name,
                    RFC = request.RFC,
                    TaxRegime = request.TaxRegime,
                    TaxRegimeName = request.TaxRegimeName,
                    CodePostal = request.CodePostal,
                    UseCFDI = request.UseCFDI,
                    UseCFDIName = request.UseCFDIName,
                    Contact = request.Contact,
                    CreateDate = request.CreateDate,
                };
                _unitOfWork.Invoice.Add(invoice);
                await _unitOfWork.SaveChangeAsync();

                var sale = await GetSales(new SaleFilter { Id = request.Sale });

                _emailService.SendCFDI(invoice, sale.FirstOrDefault());

                return invoice.Id;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar guardar factura\n{ex.Message}");
            }
        }
        public async Task<bool> PostTryInvoice(InvoiceTryREQ request) {

            try {

                //invoice
                var invoices = await GetInvoices(new InvoiceFilter { Id = request.Invoice });
                if(invoices.Count() <= 0) throw new BusinessException("No se encontro factura");

                var invoice = invoices.FirstOrDefault();
                var invoiceMap = new TbInvoice {

                    Id = invoice.Id,
                    Sale = invoice.Sale,
                    Name = invoice.Name,
                    RFC = invoice.RFC,
                    CodePostal = invoice.CodePostal,
                    Contact = invoice.Contact,
                    CreateDate = invoice.CreateDate,
                    TaxRegime = invoice.TaxRegime,
                    TaxRegimeName = invoice.TaxRegimeName,
                    UseCFDI = invoice.UseCFDI,
                    UseCFDIName = invoice.UseCFDIName
                };

                //sale
                var sale = await GetSales(new SaleFilter { Id = request.Sale });
                if(sale == null) throw new BusinessException("No se encontro venta");

                _emailService.SendCFDI(invoiceMap, sale.FirstOrDefault());

                return true;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar timbrar factura\n{ex.Message}");
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