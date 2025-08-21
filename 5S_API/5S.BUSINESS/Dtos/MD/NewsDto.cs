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
    public class NewsDto : BaseMdDto, IMapFrom, IDto
    {
        [Key]
        [Description("Mã tin tức")]
        public string Id { get; set; }
        [Description("Tên tiêu đề")]
        public string Title { get; set; }
        [Description("Ngày tạo")]
        public DateTime CreatedDate { get; set; }
        [Description("Nội dung tiêu đề")]
        public string Content { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdNews, NewsDto>().ReverseMap();
        }
    }
}
