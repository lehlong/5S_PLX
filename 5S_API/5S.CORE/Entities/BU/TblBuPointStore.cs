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
    [Table("T_BU_POINT_STORE")]
    public class TblBuPointStore : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(50)")]
        public string? Code { get; set; }

        [Column("INPUT_STORE_ID", TypeName = "NVARCHAR(50)")]
        public string? InStoreId { get; set; }

        [Column("SURVEY_ID", TypeName = "NVARCHAR(50)")]
        public string? SurveyId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(50)")]
        public string? KiKhaoSatId { get; set; }

        [Column("POINT", TypeName = "DECIMAIL(18.3)")]
        public decimal? Point { get; set; }

    }
}
