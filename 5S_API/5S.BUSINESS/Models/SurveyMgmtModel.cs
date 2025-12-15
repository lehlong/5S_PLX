using AutoMapper;
using PLX5S.BUSINESS.Dtos.BU;
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
        public List<InputDoiTuong>? InputDoiTuong { set; get; } = new List<InputDoiTuong>();
    }


    public class InputDoiTuong()
    {
        public TblBuInputDoiTuong? DoiTuong { set; get; } = new TblBuInputDoiTuong();
        public List<TblBuInputAtvsv>? Atvsvs { set; get; } = new List<TblBuInputAtvsv>();
    }


    /// Update

    public class SurveyMgmtModel2
    {
        public TblBuSurveyMgmt SurveyMgmt { set; get; } = new TblBuSurveyMgmt();
        public List<InputDoiTuong>? InputDoiTuong { set; get; } = new List<InputDoiTuong>();
    }


    public class InputDoiTuong2()
    {
        public DoiTuongModel? DoiTuong { set; get; } = new DoiTuongModel();
        public List<TblBuInputAtvsv>? Atvsvs { set; get; } = new List<TblBuInputAtvsv>();
    }

    public class DoiTuongModel()
    {
        public string Id { set; get; }
        public string DoiTuongId { set; get; }
        public string SurveyMgmtId { set; get; }
        public string Name { set; get; }
        public string ThuTruong { set; get; }
        public string ChuyenQuan { set; get; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuInputDoiTuong, DoiTuongModel>().ReverseMap();
        }
    }

}
