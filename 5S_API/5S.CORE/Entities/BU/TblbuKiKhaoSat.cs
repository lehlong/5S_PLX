using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Wordprocessing;
using PLX5S.CORE.Common;
namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_KI_KHAO_SAT")]
    public class TblBuKiKhaoSat:SoftDeleteEntity
    {
        [Key]   
        
        [Column("CODE", TypeName = "NVARCHAR(50)")]
        public string Code { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(50)")]
        public string? Name { get; set; }

        [Column("DES", TypeName = "NCHAR(50)")]
        public string? Des { get; set; }

        [Column("START_DATE")]
        
        public DateTime? StartDate { get; set; }

        [Column("SURVEY_MGMT_ID")]
        public string? SurveyMgmtId { get; set; }


        [Column("END_DATE")]
        public DateTime? EndDate { get; set; }

      

    }
}
