using AutoMapper;
using Common;
using PLX5S.CORE.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Dtos.MD
{
  
    public class AtvsvDto : IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string HeaderId { get; set; }
        public string AccountId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdAtvsv, AtvsvDto>().ReverseMap();
        }
    }
}

