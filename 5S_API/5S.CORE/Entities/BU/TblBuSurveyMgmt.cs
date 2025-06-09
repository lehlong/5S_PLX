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

    [Table("T_BU_SURVEY_MGMT")]
    public class TblBuSurveyMgmt : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string? Id { get; set; }

        [Column("MA")]
        public string? Ma { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string? Name { get; set; }

        [Column("MO_TA", TypeName = "NVARCHAR(255)")]
        public string? MoTa { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(255)")]
        public string? DoiTuongId { get; set; }

        [Column("IMAGE", TypeName = "NVARCHAR(255)")]
        public string? Image { get; set; }
    }
}
