using System;
using System.ComponentModel;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class EvaluateHeaderDto : IMapFrom, IDto
    {
        [Description("Mã")]
        public string Code { get; set; }
        public string? Name { get; set; }
        public decimal? Point { get; set; }
        public string? AccountUserName { get; set; }
        public decimal? Order { get; set; }
        public string? StoreId { get; set; }
        public string? KiKhaoSatId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreateBy { set; get; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuEvaluateHeader, EvaluateHeaderDto>().ReverseMap();
        }
    }
}
