using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class KiKhaoSatDto : IMapFrom, IDto
    {
        [Description("Mã kỳ khảo sát")]
        public string Code { get; set; }

        [Description("Tên kỳ khảo sát")]
        public string Name { get; set; }

        [Description("Mô tả")]
        public string Des { get; set; }

        [Description("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [Description("Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        [Description("header id")]
        public string Survey_Mgmt_Id { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuKiKhaoSat, KiKhaoSatDto>().ReverseMap();
        }
    }
}
