using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class KiKhaoSatDto : IMapFrom, IDto
    {
        [Description("Mã kỳ khảo sát")]
        public string Code { get; set; }

        [Description("Tên kỳ khảo sát")]
        public string Name { get; set; }

        [Description("Mô tả")]
        public string? Des { get; set; }

        [Description("Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [Description("Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        [Description("header id")]
        public string SurveyMgmtId { get; set; }
        public List<NguoiChamDiemlst> Chamdiemlst { get; set; }
        public bool? IsActive { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuKiKhaoSat, KiKhaoSatDto>().ReverseMap();
        }
    }
   public class NguoiChamDiemlst
    {
        public string Id { get; set; }
        public string storeId { get; set; }
        public string SurveyMgmtId { get; set; }
        public Storejs Store { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? DeleteBy { get; set; }
        public bool? IsActive { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<string> NguoiChamDiem { get; set; }= new List<string>();
    }
}

public class Storejs
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string CuaHangTruong { get; set; }
    public string NguoiPhuTrach { get; set; }
    public string KinhDo { get; set; }
    public string ViDo { get; set; }
    public bool? TrangThaiCuaHang { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeleteDate { get; set; }
    public string? DeleteBy { get; set; }
    public bool? IsActive { get; set; }
    public string CreateBy { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}