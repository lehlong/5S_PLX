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
    [Table("T_BU_INPUT_ATVSV")]
    public class TblBuInputAtvsv : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string? Id { get; set; }

        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string? Name { get; set; }

        [Column("INPUT_STORE_ID")]
        public string? InputStoreId { get; set; }
        [Column("TYPE")]
        public string? Type { get; set; }
    }
}
