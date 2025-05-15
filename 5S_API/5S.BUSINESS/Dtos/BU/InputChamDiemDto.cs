using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class InputChamDiemDto : IMapFrom, IDto
    {
        [Key]
        [Description("id")]
        public string Id { get; set; }
        [Description("Mã cửa hàng")]
        public string StoreId { get; set; }

        [Description("Mã kỳ khảo sát")]
        public string KiKhaoSatId { get; set; }

        [Description("Tên tài khoản")]
        public string UserName { get; set; }

        [Description("Trạng thái")]
        public bool? IsActive { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuInputChamDiem, InputChamDiemDto>().ReverseMap();
        }
    }
}
