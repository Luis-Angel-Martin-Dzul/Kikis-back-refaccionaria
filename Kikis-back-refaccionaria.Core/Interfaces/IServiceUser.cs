using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceUser {

        /*
         *  GET
         */
        Task<PagedResponse<RolRES>> GetRols(RolFilter filter);
        Task<PagedResponse<UserRES>> GetUsers(UserFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteUser(int id);


        /*
         *  POST
         */
        Task<UserRES> PostUser(UserREQ request);
        Task<UserAuthRES> Login(AuthREQ request);
        Task<bool> PostRols(RolRES request);


        /*
         *  PUT
         */
        Task<bool> PutRols(RolRES request);
        Task<UserRES> PutUser(UserREQ request);
    }
}
