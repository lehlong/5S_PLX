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
        public string LanCham { set; get; }
        public string ChuaCham { set; get; }
        public string ViPham { set; get; }
        public List<DoiTuong> LstDoiTuong { set; get; } = new List<DoiTuong>();
    }

    public class DoiTuong
    {
        public string? Id { set; get; }
        public string? Name { set; get; }
        public string? Type { set; get; }
        public decimal? Point { set; get; }
        public DateTime? FDate { set; get; }
    }
}
