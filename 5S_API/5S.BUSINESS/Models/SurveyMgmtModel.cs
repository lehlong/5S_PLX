using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class SurveyMgmtModel
    {
        public TblBuSurveyMgmt SurveyMgmt { set; get; } = new TblBuSurveyMgmt();
        public List<InputStoreModel>? InputStores { set; get; } = new List<InputStoreModel>();
        public List<InputWareHouseModel>? InputWareHouse { set; get; } = new List<InputWareHouseModel>();
    }
    public class InputStoreModel()
    {
        public TblBuInputStore InputStore { set; get; } = new TblBuInputStore();
        public List<TblBuInputAtvsv>? Atvsvs { set; get; } = new List<TblBuInputAtvsv>();
    }
    public class InputWareHouseModel()
    {
        public TblBuInputWareHouse? InputWareHouse { set; get; } = new TblBuInputWareHouse();
        public List<TblBuInputAtvsv>? Atvsvs { set; get; } = new List<TblBuInputAtvsv>();
    }
}
