using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_EVALUATE_IMAGE")]
    public class TblBuEvaluateImage : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(50)")]
        public string? Code { get; set; }

        [Column("FILE_NAME", TypeName = "NVARCHAR(50)")]
        public string? FileName { get; set; }

        [Column("FILE_PATH", TypeName = "DECIMAL(18.0)")]
        public string? FilePath { get; set; }

        [Column("TIEU_CHI_CODE", TypeName = "NVARCHAR(50)")]
        public string? TieuChiCode { get; set; }

    }
}
