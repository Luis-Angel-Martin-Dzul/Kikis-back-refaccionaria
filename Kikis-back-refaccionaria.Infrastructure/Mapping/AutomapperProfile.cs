using AutoMapper;
using Kikis_back_refaccionaria.Core.Entities;
using Kikis_back_refaccionaria.Core.Request;

namespace Kikis_back_refaccionaria.Infrastructure.Mapping {
    public class AutomapperProfile : Profile {

        public AutomapperProfile() {

            //DTO
            CreateMap<ProductREQ, TbProduct>();

            //Entity
            CreateMap<TbProduct, ProductREQ>();

        }

    }
}
