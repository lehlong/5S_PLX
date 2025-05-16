using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Entities.BU
{
    
    [Table("T_BU_TIEU_CHI")]
public class TblBuTieuChi : SoftDeleteEntity
{
        [Key]
        [Column("ID", TypeName = "VARCHAR(50)")]
        public string Id { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }

        [Column("PARENT_ID", TypeName = "VARCHAR(50)")]
        public string? PId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "VARCHAR(50)")]
        public string? KiKhaoSatId { get; set; }

        [Column ("IS_GROUP")]
        public bool? IsGroup { get; set; }

        [Column("IS_IMG")]
        public bool? IsImg { get; set; }

        [Column("ORDER_NUMBER")]
        public int OrderNumber { get; set; }

        [Column("REPORT", TypeName = "NVARCHAR(255)")]
        public string Report { get; set; }
    }
}
