using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using PLX5S.BUSINESS.Dtos.AD;
using PLX5S.CORE.Entities.AD;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class TieuChiDto : IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string? PId { get; set; }

        public string? KiKhaoSatId { get; set; }

        public int? OrderNumber { get; set; }

        public bool? IsGroup { get; set; }
        public bool? IsImg { get; set; }

        public string Report { get; set; }


        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuTieuChi, TieuChiDto>().ReverseMap();
        }
    }
}
