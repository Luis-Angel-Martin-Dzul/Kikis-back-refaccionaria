using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Exceptions;
using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Kikis_back_refaccionaria.Infrastructure.Encryption;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Repositories {
    public class ServiceUser : IServiceUser {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceEMail _emailService;
        public ServiceUser(IUnitOfWork unitOfWork, IServiceEMail emailService) {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

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
    }
}
