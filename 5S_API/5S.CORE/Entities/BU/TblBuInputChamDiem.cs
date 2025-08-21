using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PLX5S.CORE.Common;
using PLX5S.CORE.Entities.MD;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_INPUT_CHAM_DIEM")]
    public class TblBuInputChamDiem : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        //[Column("STORE_ID", TypeName = "NVARCHAR(50)")]
        //public string InStoreId { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(50)")]
        public string? DoiTuongId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(50)")]
        public string KiKhaoSatId { get; set; }

        [Column("USER_NAME", TypeName = "NVARCHAR(50)")]
        public string UserName { get; set; }

        //[ForeignKey("StoreId")]
        //public virtual TblMdStore Store { get; set; }


    }
}

