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
}
