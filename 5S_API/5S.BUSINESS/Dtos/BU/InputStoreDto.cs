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
        public string Ma { get; set; }

        [Description("Mã kỳ khảo sát")]
        public string SurveyMgmtId { get; set; }

        [Description("Tên cửa hàng")]
        public string Name { get; set; }

        [Description("Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Description("Cửa hàng trưởng")]
        public string CuaHangTruong { get; set; }

        [Description("Người phụ trách")]
        public string NguoiPhuTrach { get; set; }

        [Description("Kinh độ")]
        public string KinhDo { get; set; }

        [Description("Vĩ độ")]
        public string ViDo { get; set; }

        [Description("Trạng thái cửa hàng")]
        public bool? TrangThaiCuaHang { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuInputStore, InputStoreDto>().ReverseMap();
        }
    }
}
