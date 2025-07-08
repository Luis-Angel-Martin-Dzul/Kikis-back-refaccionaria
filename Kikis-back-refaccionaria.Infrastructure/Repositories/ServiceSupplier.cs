using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceSupplier : IServiceSupplier {

        private readonly IUnitOfWork _unitOfWork;
        public ServiceSupplier(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }


        /*
         *  GET
         */
        public async Task<PagedResponse<SupplierRES>> GetSupplier(PaginationFilter filter) {

            var query = _unitOfWork.Supplier
                .GetQuery()
                .AsNoTracking();

            int totalItems = await query.CountAsync();

            var suppliers = await query
                .OrderBy(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new SupplierRES {
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
            }).ToListAsync();

            //response
            return new PagedResponse<SupplierRES> {
                Items = suppliers,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
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
    }
}
