using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PLX5S.CORE.Common;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_INPUT_CHAM_DIEM")]
    public class TblBuInputChamDiem : SoftDeleteEntity
    {
    
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("STORE_ID", TypeName = "NVARCHAR(50)")]
        public string StoreId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(50)")]
        public string KiKhaoSatId { get; set; }

        [Column("USER_NAME", TypeName = "NVARCHAR(50)")]
        public string UserName { get; set; }

    
    }
}

