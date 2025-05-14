using PLX5S.CORE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.CORE.Entities.MD
{
    [Table("T_MD_ATVSV")]
    public class TblMdAtvsv : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        [Column("NAME", TypeName = "NVARCHAR(255)")]
        public string Name { get; set; }
        [Column("HEADER_ID")]
        public string HeaderId { get; set; }
        [Column("ACCOUNT_ID")]
        public string AccountId { get; set; }

    }
}
