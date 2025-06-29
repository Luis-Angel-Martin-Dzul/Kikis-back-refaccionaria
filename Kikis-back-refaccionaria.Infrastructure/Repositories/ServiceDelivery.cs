using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceDelivery : IServiceDelivery {

        private readonly IUnitOfWork _unitOfWork;
        public ServiceDelivery(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

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
                foreach(var delivery in dDeliveryDetails)
                    delivery.Status = 1; //Creado

                _unitOfWork.DeliveryDetail.UpdateRange(dDeliveryDetails);


                var AdeliveryIds = news.Select(d => d.Delivery).ToList();
                var AdeliveryDetails = await _unitOfWork.DeliveryDetail
                    .GetQuery()
                    .Where(d => AdeliveryIds.Contains(d.Id))
                    .ToListAsync();
                foreach(var delivery in AdeliveryDetails)
                    delivery.Status = 2; //Pendiente

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
    }
}
