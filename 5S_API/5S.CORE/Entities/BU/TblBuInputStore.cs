using PLX5S.CORE.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_INPUT_STORE")]
    public class TblBuInputStore : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("STORE_ID", TypeName = "NVARCHAR(50)")]
        public string StoreId { get; set; }

        [Column("SURVEY_MGMT_ID", TypeName = "NVARCHAR(50)")]
        public string SurveyMgmtId { get; set; }

    }
}
