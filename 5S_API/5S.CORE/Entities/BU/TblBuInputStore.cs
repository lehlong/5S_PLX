using PLX5S.CORE.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_INPUT_STORE")]
    public class TblBuInputStore : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("MA", TypeName = "NVARCHAR(50)")]
        public string Ma { get; set; }

        [Column("SURVEY_MGMT_ID", TypeName = "NVARCHAR(50)")]
        public string SurveyMgmtId { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(18)")]
        public string Name { get; set; }

        [Column("PHONE_NUMBER", TypeName = "NVARCHAR(30)")]
        public string PhoneNumber { get; set; }

        [Column("CUA_HANG_TRUONG", TypeName = "NVARCHAR(200)")]
        public string CuaHangTruong { get; set; }

        [Column("NGUOI_PHU_TRACH", TypeName = "NVARCHAR(150)")]
        public string NguoiPhuTrach { get; set; }

        [Column("KINH_DO", TypeName = "NVARCHAR(120)")]
        public string KinhDo { get; set; }

        [Column("VI_DO", TypeName = "NVARCHAR(150)")]
        public string ViDo { get; set; }

        [Column("TRANG_THAI_CUA_HANG", TypeName = "BIT")]
        public bool? TrangThaiCuaHang { get; set; }
    }
}
