using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class FilterReport
    {
        public string? KiKhaoSatId { set; get; }
        public string? SurveyId { set; get; }
        public string? DoiTuongId { set; get; }

    }


    public class KetQuaChamDiem
    {
        public string stt { set; get; }
        public string Name { set; get; }
        public decimal? Length { set; get; }
        public decimal? point { set; get; }
    }

    public class ThoiGianChamDiem
    {
        public string stt { set; get; }
        public string Name { set; get; }
        public decimal? Point { set; get; }
        public List<string>? Cht { set; get; }
        public List<string>? Atvsv { set; get; }
        public List<string>? ChuyenGia { set; get; }
    }


    public class TheoKhungThoiGian
    {
        public string stt { set; get; }
        public string Name { set; get; }
        public decimal? Cht_T { set; get; }
        public decimal? Cht_N { set; get; }
        public decimal? Atvsv_T { set; get; }
        public decimal? Atvsv_N { set; get; }
        public decimal? ChuyenGia { set; get; }
        public decimal? Point { set; get; }
    }

    public class TongHopYKienDeXuat
    {
        public string? stt { set; get; }
        public string? Name { set; get; }
        public List<LstTieuChiDeXuat>? lstTieuChiDeXuat { set; get; }
    }
    public class LstTieuChiDeXuat
    {
        public string? TieuChi { set; get; }
        public string? DeXuat { set; get; }
        public string? CanBo { set; get; }
        public string? ChucVu { set; get; }
        public string? ThoiGian { set; get; }

    }

}
