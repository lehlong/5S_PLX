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
    public class DeviceDto : BaseMdDto, IMapFrom, IDto
    {

        [Key]
        [Description("ID")]
        public string Id { get; set; }

        [Description("Tên đăng nhập")]
        public string UserName { get; set; }

        [Description("ID thiết bị")]
        public string DeviceId { get; set; }

        [Description("Tên thiết bị")]
        public string DeviceName { get; set; }

        [Description("Hệ điều hành")]
        public string OperatingSystem { get; set; }

        [Description("Model")]
        public string Model { get; set; }

        [Description("Nhà sản xuất")]
        public string Manufacturer { get; set; }

        [Description("Phiên bản HĐH")]
        public string OsVersion { get; set; }
        [Description("Thiết bị chính")]
        public bool MainDevice { get; set; }
        [Description("Cho phép đăng nhập")]
        public bool EnableLogin { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdDevice, DeviceDto>().ReverseMap();
        }
    }
}
