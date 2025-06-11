using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_EVALUATE_VALUE")]
    public class TblBuEvaluateValue : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(150)")]
        public string? Code { get; set; }

        [Column("EVALUATE_HEADER_CODE", TypeName = "NVARCHAR(150)")]
        public string? EvaluateHeaderCode { get; set; }

        [Column("POINT_ID", TypeName = "NVARCHAR(150)")]
        public string? PointId { get; set; }

        [Column("TIEU_CHI_CODE", TypeName = "NVARCHAR(150)")]
        public string? TieuChiCode { get; set; }

        [Column("FEED_BACK", TypeName = "NVARCHAR(1500)")]
        public string? FeedBack { get; set; }

    }
}
