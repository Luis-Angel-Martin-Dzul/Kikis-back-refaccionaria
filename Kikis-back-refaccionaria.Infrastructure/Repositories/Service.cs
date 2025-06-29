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
using System.Linq;

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
