using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;
using PLX5S.CORE.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class InputAtvsvDto : IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string StoreId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuInputAtvsv, InputAtvsvDto>().ReverseMap();
        }
    }
}
