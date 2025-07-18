﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLX5S.CORE.Common;

namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_CRITERIA_EXCLUDED_Object")]
    public class TblBuCriteriaExcludedObject : SoftDeleteEntity
    {
        [Key]
        [Column("CODE")]
        public string? Code { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(255)")]
        public string? DoiTuongId { get; set; }

        [Column("TIEU_CHI_CODE")]
        public string? TieuChiCode { get; set; }
    }
}
