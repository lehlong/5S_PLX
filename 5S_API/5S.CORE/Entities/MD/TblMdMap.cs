
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLX5S.CORE.Common;

namespace PLX5S.CORE.Entities.MD
{

    [Table("T_MD_MAP")]
    public class TblMdMap : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }

        [Column("ADDRESS", TypeName = "NVARCHAR(255)")]
        public string? Address { get; set; }

        [Column("DESCRIPTION", TypeName = "NVARCHAR(255)")]
        public string? Description { get; set; }

        [Column("KINH_DO", TypeName = "DECIMAL(18.10)")]
        public decimal? KinhDo { get; set; }

        [Column("VI_DO", TypeName = "DECIMAL(18.10)")]
        public decimal? ViDo { get; set; }

    }
}
