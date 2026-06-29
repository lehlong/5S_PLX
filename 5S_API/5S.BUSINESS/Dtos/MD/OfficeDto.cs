using AutoMapper;
using Common;
using PLX5S.CORE.Entities.MD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Dtos.MD
{
    public class OfficeDto : BaseMdDto, IMapFrom, IDto
    {

        [Key]
        [Description("Mã kiểu người dùng")]
        public string Id { get; set; }

        [Description("Tên kiểu người dùng")]
        public string Name { get; set; }

        [Description("Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Description("Thủ trưởng")]
        public string ThuTruong { get; set; }

        [Description("Người phụ trách")]
        public string NguoiPhuTrach { get; set; }

        [Description("Địa chỉ")]
        public string? Address { get; set; }

        [Description("Kinh độ")]
        public string? KinhDo { get; set; }

        [Description("Vĩ độ")]
        public string? ViDo { get; set; }

        [Description("Trạng Thái")]
        public bool TrangThai { get; set; }

        public List<string> ATVSV { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdOffice, OfficeDto>().ReverseMap();
        }
    }
}
