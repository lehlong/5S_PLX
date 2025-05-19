using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class TinhDiemTieuChiDto : BaseMdDto, IMapFrom, IDto
    {
        [Key]
        public string Id { get; set; }
        public string MoTa { get; set; }
        public decimal Diem { get; set; }
        public string TieuChiId { get; set; }

        [Description("Trạng thái")]
        public string State { get => this.IsActive == true ? "Đang hoạt động" : "Khóa"; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuTinhDiemTieuChi, TinhDiemTieuChiDto>().ReverseMap();
        }
    }
}