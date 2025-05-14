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
        [Description("Mã chức vụ")]
        public string Id { get; set; }
        [Description("Tên chức vụ")]
        public string Name { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuSurveyMgmt, SurveyMgmtDto>().ReverseMap();
        }
    }
}