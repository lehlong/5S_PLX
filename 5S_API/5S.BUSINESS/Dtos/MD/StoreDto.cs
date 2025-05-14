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
    public class StoreDto : BaseMdDto, IMapFrom, IDto
    {
       
        [Key]
        [Description("Mã kiểu người dùng")]
        public string Id { get; set; }
        [Description("Tên kiểu người dùng")]
        public string Name { get; set; }
        [Description("Số điện thoại")]
        public string Phone { get; set; }
        [Description("Cửa hàng trưởng")]
        public string CuaHangTruong { get; set; }
        [Description("Người phụ trách")]
        public string NguoiPhuTrach { get; set; }
        [Description("Kinh độ")]
        public string KinhDo { get; set; }
        [Description("Vĩ độ")]
        public string ViDo { get; set; }
        [Description("Trạng Thái cửa hàng")]
        public bool TrangThaiCuaHang { get; set; }
        public List<string> ATVSV { get; set; }


        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdStore, StoreDto>().ReverseMap();
        }
    }
}
