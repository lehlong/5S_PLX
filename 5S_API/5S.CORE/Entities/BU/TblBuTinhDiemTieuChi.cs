using PLX5S.CORE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_TINH_DIEM_TIEU_CHI")]
    public class TblBuTinhDiemTieuChi : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string? Id { get; set; }

        [Column("MO_TA", TypeName = "NVARCHAR(50)")]
        public string MoTa { get; set; }

        [Column("DIEM", TypeName = "DECIMAL(18.0)")]
        public decimal Diem { get; set; }

        [Column("TIEU_CHI_CODE", TypeName = "NVARCHAR(50)")]
        public string? TieuChiCode { get; set; }
    }
}
