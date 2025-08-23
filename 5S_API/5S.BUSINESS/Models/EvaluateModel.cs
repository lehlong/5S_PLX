using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class EvaluateModel
    {
        public TblBuEvaluateHeader? Header { set; get; } = new TblBuEvaluateHeader();
        public List<TblBuEvaluateValue>? LstEvaluate { set; get; } = new List<TblBuEvaluateValue>();
        public List<TblBuEvaluateImage>? LstImages { set; get; } = new List<TblBuEvaluateImage>();
    }


    public class HomeModel
    {
        public decimal? LanCham { set; get; } = 0;
        public decimal? ChuaCham { set; get; } = 0;
        public decimal? ViPham { set; get; } = 0;
        public List<DoiTuong> LstDoiTuong { set; get; } = new List<DoiTuong>();
    }

    public class DoiTuong
    {
        public string? Id { set; get; }
        public string? KiKhaoSatCode { set; get; }
        public string? Name { set; get; }
        public string? Type { set; get; }
        public bool? IsScore { set; get; } = false;
        public string? KiKhaoSatName { set; get; }
        public string? SurveyId { set; get; }
        public string? KiKhaoSatId { set; get; }
        public decimal? Point { set; get; }
        public DateTime? FDate { set; get; }
        public DateTime? EndDate { set; get; }
        public List<string>? LstChamDiem { set; get; }
    }
}
