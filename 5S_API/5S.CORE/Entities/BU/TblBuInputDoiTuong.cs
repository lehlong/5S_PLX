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
    [Table("T_BU_INPUT_DOI_TUONG")]
    public class TblBuInputDoiTuong : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(50)")]
        public string DoiTuongId { get; set; }

        [Column("SURVEY_MGMT_ID", TypeName = "NVARCHAR(50)")]
        public string SurveyMgmtId { get; set; }

    }
}
