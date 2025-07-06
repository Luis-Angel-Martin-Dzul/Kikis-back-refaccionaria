using AutoMapper;
using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Kikis_back_refaccionaria.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceProduct : IServiceProduct {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ServiceProduct(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /*
         *  GET
         */
        public async Task<PagedResponse<ProductRES>> GetProducts(ProductFilter filter, string schema) {

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

            int totalItems = await query.CountAsync();

            //select
            var products = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new ProductRES {
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
                        Name = x.KitNavigation.Name,
                    }).ToList(),
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Discount = x.Discount,
                    Path = schema + x.Path,
                    IsActive = x.IsActive
            }).ToListAsync();

            //response
            return new PagedResponse<ProductRES> {
                Items = products,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
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
        public async Task<ProductRES> PostProduct(ProductREQ request, string schema) {

            try {

                var product = _mapper.Map<TbProduct>(request);

                //product
                product.Path = Util.SaveBase64File(request, Constants.PATH_IMG_PRODUCT);
                _unitOfWork.Product.Add(product);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetProducts(new ProductFilter { Id = product.Id }, schema);
                var productRES = lastInsert.Items.FirstOrDefault();


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
    }
}
