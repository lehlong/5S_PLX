using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class SurveyMgmtDto : BaseMdDto, IMapFrom, IDto
    {
        [Key]
        [Description("Mã")]
        public string Id { get; set; }

        [Description("Mã")]
        public string? Ma { get; set; }

        [Description("Tên")]
        public string Name { get; set; }

        [Description("mô tả")]
        public string MoTa { get; set; }

        [Description("mã đối tượng")]
        public string DoiTuongId { get; set; }

        [Description("Ảnh")]
        public string? Image { get; set; }

        //[Dsescription("Tên chức vụ")]
        //public List<TblBuInputStore> InputStore { get; set; } = new List<TblBuInputStore>();


        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuSurveyMgmt, SurveyMgmtDto>().ReverseMap();
        }
    }
}