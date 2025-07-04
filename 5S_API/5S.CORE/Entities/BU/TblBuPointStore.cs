﻿using PLX5S.CORE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PLX5S.CORE.Entities.BU
{
    [Table("T_BU_POINT")]
    public class TblBuPoint : SoftDeleteEntity
    {
        [Key]
        [Column("CODE", TypeName = "NVARCHAR(50)")]
        public string? Code { get; set; }

        [Column("DOI_TUONG_ID", TypeName = "NVARCHAR(50)")]
        public string? DoiTuongId { get; set; }

        [Column("SURVEY_ID", TypeName = "NVARCHAR(50)")]
        public string? SurveyId { get; set; }

        [Column("KI_KHAO_SAT_ID", TypeName = "NVARCHAR(50)")]
        public string? KiKhaoSatId { get; set; }

        [Column("POINT", TypeName = "DECIMAIL(18.3)")]
        public decimal? Point { get; set; }

        [Column("LENGTH", TypeName = "DECIMAIL(18.0)")]
        public decimal? Length { get; set; }
    }
}
