using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class KiKhaoSatModel
    {
        public TblBuKiKhaoSat KiKhaoSat { set; get; } = new TblBuKiKhaoSat();
        public List<InputStore> lstInputStore { set; get; } = new List<InputStore>();

    }

    public class InputStore
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string CuaHangTruong { get; set; }
        public string NguoiPhuTrach { get; set; }
        public string? ViDo { get; set; }
        public string? KinhDo { get; set; }
        public bool TrangThaiCuaHang { get; set; }
        public string StoreId { get; set; }
        public string SurveyMgmtId { get; set; }
        public List<string>? LstChamDiem { set; get; } = new List<string>();
        public List<TblBuInputChamDiem>? LstInChamDiem { set; get; } = new List<TblBuInputChamDiem>();
    }
}
