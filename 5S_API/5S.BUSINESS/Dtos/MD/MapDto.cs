
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.MD;

namespace PLX5S.BUSINESS.Dtos.MD
{
    public class MapDto : BaseMdDto, IMapFrom, IDto
    {
        [Key]
        [Description("Mã đối tượng")]
        public string? Id { get; set; }
        
        [Description("Tên đối tượng")]
        public string Name { get; set; }

        [Description("Vị trí")]
        public string? Address { get; set; }

        [Description("Mô tả")]
        public string? Description { get; set; }

        [Description("Kinh độ")]
        public decimal KinhDo { get; set; }

        [Description("Vĩ độ")]
        public decimal ViDo { get; set; }
        public decimal? KhoangCach { get; set; }

        [Description("Trạng thái")]
        public string? State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdMap, MapDto>().ReverseMap();
        }
    }
}
