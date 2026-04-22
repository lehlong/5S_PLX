using PLX5S.CORE.Common;
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
    [Table("T_BU_USER_TOKEN_NOTI")]
    public class TblBuUserTokenNoti : SoftDeleteEntity
    {
        [Key]
        [Column("ID", TypeName = "NVARCHAR(50)")]
        public string Id { get; set; }

        [Column("USER_NAME", TypeName = "NVARCHAR(50)")]
        public string UserName { get; set; }

        [Column("TOKEN", TypeName = "NVARCHAR(500)")]
        public string Token { get; set; }

    }
}
