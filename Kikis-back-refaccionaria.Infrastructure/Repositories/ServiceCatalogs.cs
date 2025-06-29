using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceCatalogs : IServiceCatalogs {

        private readonly IUnitOfWork _unitOfWork;
        public ServiceCatalogs(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        /*
         *  GET
         */
        public async Task<IEnumerable<GenericCatalog>> GetProductCategory() {
            var data = _unitOfWork.ProductCategory
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

        public async Task<IEnumerable<GenericCatalog>> GetProductHallway() {

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

        public async Task<IEnumerable<GenericCatalog>> GetProductLevel() {

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
    }
}
