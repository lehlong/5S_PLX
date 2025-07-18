﻿using System;
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
    public class WarehouseDto : BaseMdDto, IMapFrom, IDto
    {

        [Key]
        [Description("Mã ")]
        public string Id { get; set; }

        [Description("Tên kho")]
        public string Name { get; set; }

        [Description("Trưởng kho")]
        public string TruongKho { get; set; }

        [Description("Người phụ trách")]
        public string NguoiPhuTrach { get; set; }

        [Description("Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Description("Địa chỉ")]
        public string? Address { get; set; }

        [Description("Trạng Thái Kho")]
        public bool TrangThaiKho { get; set; }

        public List<string> ATVSV { get; set; }
        
        [Description("Kinh do")]
        public string? KinhDo { get; set; }
        
        [Description("Vi do")]
        public string? ViDo { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblMdWareHouse, WarehouseDto>().ReverseMap();
        }
    }
}
