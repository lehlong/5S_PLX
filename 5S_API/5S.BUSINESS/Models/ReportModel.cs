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
        public string? AccountUserName { set; get; }
        public string? InstoreId { set; get; }

    }


    public class KetQuaChamDiem
    {
        public string stt { set; get; }
        public string StoreName { set; get; }
        public decimal? Length { set; get; }
        public decimal? point { set; get; }
    }

    public class ThoiGianChamDiem
    {
        public string stt { set; get; }
        public string StoreName { set; get; }
        public decimal? Point { set; get; }
        public List<string>? Cht { set; get; }
        public List<string>? Atvsv { set; get; }
        public List<string>? ChuyenGia { set; get; }
    }
}
