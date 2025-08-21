using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLX5S.CORE.Common;

namespace PLX5S.CORE.Entities.MD
{

    [Table("T_MD_NEWS")]
    public class TblMdNews : SoftDeleteEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        [Column("TITLE", TypeName = "NVARCHAR(255)")]
        public string Title { get; set; }
        [Column("CONTENT", TypeName = "NVARCHAR(MAX)")]
        public string Content { get; set; }
        [Column("CREATED_DATE", TypeName = "DATETIME")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
