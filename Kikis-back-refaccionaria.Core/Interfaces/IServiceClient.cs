using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceClient {

        /*
         *  GET
         */
        Task<IEnumerable<ClientRES>> GetClients(ClientFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteClient(int id);



        /*
         *  POST
         */
        Task<ClientRES> PostClient(ClientREQ request);



        /*
         *  PUT
         */
        Task<ClientRES> PutClient(ClientREQ request);

    }
}
