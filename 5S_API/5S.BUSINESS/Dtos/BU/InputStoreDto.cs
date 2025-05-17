using System;
using System.ComponentModel;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class InputStoreDto : IMapFrom, IDto
    {
        [Description("Mã cửa hàng")]
        public string Id { get; set; }

        [Description("Mã cửa hàng")]
        public string StoreId { get; set; }

        [Description("Mã kỳ khảo sát")]
        public string SurveyMgmtId { get; set; }


        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuInputStore, InputStoreDto>().ReverseMap();
        }
    }
}
