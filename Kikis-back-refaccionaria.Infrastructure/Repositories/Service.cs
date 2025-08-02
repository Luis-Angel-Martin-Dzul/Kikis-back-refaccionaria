using AutoMapper;
using Kikis_back_refaccionaria.Core.Interfaces;

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
