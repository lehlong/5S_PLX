using PLX5S.CORE.Entities.BU;

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
        public string? KiKhaoSatName { set; get; }
        public string? SurveyId { set; get; }
        public string? KiKhaoSatId { set; get; }
        public string? TimeText { set; get; }
        public string? Description { set; get; }
        public bool? IsScore { set; get; } = false;
        public bool? Scored { set; get; } = false;
        public decimal? Point { set; get; }
        public decimal? viPham { set; get; }
        public DateTime? FDate { set; get; }
        public DateTime? EndDate { set; get; }
        public List<string>? LstChamDiem { set; get; }
    }

    public record DotInfo(int Dot, DateTime FDate, DateTime EDate);
}
