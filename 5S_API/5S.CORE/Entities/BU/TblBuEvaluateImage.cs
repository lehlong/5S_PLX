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
        [Column("CODE", TypeName = "NVARCHAR(150)")]
        public string? Code { get; set; }

        [Column("FILE_NAME", TypeName = "NVARCHAR(500)")]
        public string? FileName { get; set; }

        [Column("FILE_PATH", TypeName = "NVARCHAR(5000)")]
        public string? FilePath { get; set; }

        [Column("PATH_THUMBNAIL", TypeName = "NVARCHAR(5000)")]
        public string? PathThumbnail { get; set; }

        [Column("NAME_THUMBNAIL", TypeName = "NVARCHAR(500)")]
        public string? NameThumbnail { get; set; }

        [Column("TIEU_CHI_CODE", TypeName = "NVARCHAR(150)")]
        public string? TieuChiCode { get; set; }

        [Column("EVALUATE_HEADER_CODE", TypeName = "NVARCHAR(150)")]
        public string? EvaluateHeaderCode { get; set; }

        [Column("KINH_DO", TypeName = "DECIMAL(18.10)")]
        public decimal? KinhDo { get; set; }

        [Column("VI_DO", TypeName = "DECIMAL(18.10)")]
        public decimal? ViDo { get; set; }

        [Column("TYPE", TypeName = "NVARCHAR(150)")]
        public string? Type { get; set; }
    }
}
