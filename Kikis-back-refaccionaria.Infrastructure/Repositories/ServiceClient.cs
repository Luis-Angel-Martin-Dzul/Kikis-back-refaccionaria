using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceClient : IServiceClient {

        private readonly IUnitOfWork _unitOfWork;
        public ServiceClient(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }


        /*
         *  GET
         */
        public async Task<IEnumerable<ClientRES>> GetClients(ClientFilter filter) {

            //query
            var query = _unitOfWork.Client
                .GetQuery()
                .Where(x => x.IsActive == true)
                .AsNoTracking();

            //filter
            if(filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);

            if(!string.IsNullOrWhiteSpace(filter.Name)) {
                var nameToSearch = filter.Name.Trim().ToLower();

                query = query.Where(x =>
                    (x.FirstName + " " + x.LastName).ToLower().Contains(nameToSearch) ||
                    (x.LastName + " " + x.FirstName).ToLower().Contains(nameToSearch));
            }

            var clients = await query.ToListAsync();

            // Agrupar
            var response = clients.Select(x => new ClientRES {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Cellphone = x.Cellphone,
                Email = x.Email,
                Address = x.Address,
            }).ToList();

            return response;
        }


        /*
         *  DELETE
         */
        public async Task<bool> DeleteClient(int id) {

            try {

                var client = await _unitOfWork.Client.GetById(id);
                if(client == null)
                    throw new BusinessException("Cliente no encontrado");

                client.IsActive = false;

                _unitOfWork.Client.Update(client);
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
        public async Task<ClientRES> PostClient(ClientREQ request) {

            try {

                var client = new TbClient {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Cellphone = request.Cellphone,
                    Address = request.Address,
                    IsActive = true,
                };

                _unitOfWork.Client.Add(client);
                await _unitOfWork.SaveChangeAsync();

                var lastInsert = await GetClients(new ClientFilter { Id = client.Id });
                var response = lastInsert.FirstOrDefault();

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar agregar cliente\n{ex.Message}");
            }
        }


        /*
         *  PUT
         */
        public async Task<ClientRES> PutClient(ClientREQ request) {

            try {

                var client = await _unitOfWork.Client.GetById(request.Id);
                client.FirstName = request.FirstName;
                client.LastName = request.LastName;
                client.Email = request.Email;
                client.Cellphone = request.Cellphone;
                client.Address = request.Address;

                _unitOfWork.Client.Update(client);
                await _unitOfWork.SaveChangeAsync();

                var response = new ClientRES {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Cellphone = request.Cellphone,
                    Address = request.Address
                };

                return response;
            }
            catch(Exception ex) {

                throw new BusinessException($"Ocurrió un error inesperado al intentar actualizar cliente\n{ex.Message}");
            }
        }
    }
}
