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
    [Table("T_BU_NOTIFICATION")]
    public class TblBuNotification : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("TITLE", TypeName = "NVARCHAR(500)")]
        public string? Title { get; set; }

        [Column("BODY", TypeName = "NVARCHAR(1050)")]
        public string? Body { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(100)")]
        public string? KiKhaoSatId { get; set; }

        [Column("LINK", TypeName = "NVARCHAR(100)")]
        public string? Link { get; set; }

        [Column("SURVEY_ID", TypeName = "NVARCHAR(100)")]
        public string? SurveyId { get; set; }



    }
}
