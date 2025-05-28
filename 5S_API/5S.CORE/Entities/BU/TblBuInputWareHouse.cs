using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_INPUT_WARE_HOUSE")]
    public class TblBuInputWareHouse : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("WARE_HOUSE_ID", TypeName = "NVARCHAR(50)")]
        public string WareHouseId { get; set; }

        [Column("SURVEY_MGMT_ID", TypeName = "NVARCHAR(50)")]
        public string SurveyMgmtId { get; set; }

    }
}
