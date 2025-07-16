using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_EVALUATE_HEADER")]
    public class TblBuEvaluateHeader : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(150)")]
        public string? Code { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(150)")]
        public string? Name { get; set; }

        [Column("POINT", TypeName = "DECIMAL(18.0)")]
        public decimal Point { get; set; }

        [Column("ACCOUNT_USER_NAME", TypeName = "NVARCHAR(150)")]
        public string? AccountUserName { get; set; }

        [Column("ORDER_NUMBER", TypeName = "DECIMAL(18.0)")]
        public decimal? Order { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(150)")]
        public string? DoiTuongId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(150)")]
        public string? KiKhaoSatId { get; set; }

        [Column("DEVICE_ID", TypeName = "NVARCHAR(150)")]
        public string? DeviceId { get; set; }

        [Column("CHUC_VU_ID", TypeName = "NVARCHAR(150)")]
        public string? ChucVuId { get; set; }
    }
}
